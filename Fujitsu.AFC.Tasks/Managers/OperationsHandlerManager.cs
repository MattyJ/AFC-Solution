using System;
using System.Linq;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Enumerations;
using Fujitsu.AFC.Extensions;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Tasks.Properties;
using Fujitsu.AFC.Tasks.Resources;
using Fujitsu.Exceptions.Framework;


namespace Fujitsu.AFC.Tasks.Managers
{
    public class OperationsHandlerManager : TaskHandlerManager
    {
        private readonly ITaskService _taskService;
        private readonly IService<HistoryLog> _historyLogService;
        private readonly IParameterService _parameterService;
        private readonly IObjectBuilder _objectBuilder;
        private readonly ILoggingManager _loggingManager;
        private readonly IUserIdentity _userIdentity;

        public OperationsHandlerManager(ITaskService taskService,
            IService<HistoryLog> historyLogService,
            IParameterService parameterService,
            IObjectBuilder objectBuilder,
            ILoggingManager loggingManager,
            IUserIdentity userIdentity)
        {
            if (taskService == null)
            {
                throw new ArgumentNullException(nameof(taskService));
            }
            if (historyLogService == null)
            {
                throw new ArgumentException(nameof(historyLogService));
            }
            if (parameterService == null)
            {
                throw new ArgumentException(nameof(parameterService));
            }
            if (objectBuilder == null)
            {
                throw new ArgumentNullException(nameof(objectBuilder));
            }
            if (loggingManager == null)
            {
                throw new ArgumentNullException(nameof(loggingManager));
            }
            if (userIdentity == null)
            {
                throw new ArgumentNullException(nameof(userIdentity));
            }

            _taskService = taskService;
            _historyLogService = historyLogService;
            _parameterService = parameterService;
            _objectBuilder = objectBuilder;
            _loggingManager = loggingManager;
            _userIdentity = userIdentity;
        }

        public override void Execute()
        {
            try
            {
                var serviceInstanceId = Settings.Default.ServiceInstanceId;

                // Check that the Service Instance Id is not empty
                if (serviceInstanceId == Guid.Empty)
                {
                    throw new ArgumentNullException(string.Format(TaskResources.ConfigurationFile_SettingMissing, nameof(serviceInstanceId)));
                }

                // Get all outstanding operations tasks
                var tasks = _taskService.AllOperationsHandlerTasks().ToList();

                if (tasks.Any())
                {
                    foreach (var task in tasks)
                    {
                        try
                        {
                            RetryableOperation.Invoke(
                                ExceptionPolicies.General,
                                () =>
                                {
                                    // Create a new DI container to keep all work with the handler discrete
                                    _objectBuilder.AddChildContainer();

                                    // Instantiate the task handler
                                    using (var taskHandler = _objectBuilder.Resolve<ITaskHandler>(task.Handler))
                                    {
                                        // Check that the task is scheduled to be executed
                                        if (!taskHandler.CanExecute(task, serviceInstanceId))
                                        {
                                            return;
                                        }

                                        taskHandler.Execute(task, serviceInstanceId);
                                    }
                                });
                        }
                        catch (UnRecoverableErrorException)
                        {
                            // Already handled
                        }
                        catch (Exception ex)
                        {
                            // Create a History Log
                            var historyLog = task.CreateHistoryLog(_userIdentity.Name);
                            historyLog.EventType = LoggingEventTypeNames.Error;
                            historyLog.EventDetail = string.Format(TaskResources.UnexpectedApplicationErrorInstanceHandlingId, ex.Message);
                            historyLog.Escalated = false;
                            _historyLogService.Create(historyLog);

                            // Update Task NextScheduledDate
                            task.NextScheduledDate = DateTime.Now.AddMinutes(_parameterService.GetParameterByNameAndCache<int>(ParameterNames.RecoverableExceptionRetryDelayIntervalInMinutes));
                            _taskService.Update(task);
                        }
                        finally
                        {
                            _objectBuilder.RemoveAllChildContainers();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _loggingManager.Write(LoggingEventSource.OperationsService, LoggingEventType.Error, ex.Message);
            }
        }
    }
}
