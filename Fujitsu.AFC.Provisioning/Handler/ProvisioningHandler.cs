using System;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Handlers;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Enumerations;
using Fujitsu.AFC.Extensions;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Provisioning.Interfaces;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Tasks.Resources;
using Fujitsu.Exceptions.Framework;

namespace Fujitsu.AFC.Provisioning.Handler
{
    public class ProvisioningHandler : TaskHandler
    {
        private readonly IObjectBuilder _objectBuilder;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITaskService _taskService;
        private readonly IService<HistoryLog> _historyLogService;
        private readonly ILoggingManager _loggingManager;
        private readonly IUserIdentity _userIdentity;

        public ProvisioningHandler(
            IObjectBuilder objectBuilder,
            ITaskService taskService,
            IService<HistoryLog> historyLogService,
            ILoggingManager loggingManager,
            IUserIdentity userIdentity,
            IUnitOfWork unitOfWork)
        {
            if (objectBuilder == null)
            {
                throw new ArgumentNullException(nameof(objectBuilder));
            }
            if (taskService == null)
            {
                throw new ArgumentNullException(nameof(taskService));
            }
            if (historyLogService == null)
            {
                throw new ArgumentNullException(nameof(historyLogService));
            }
            if (loggingManager == null)
            {
                throw new ArgumentNullException(nameof(loggingManager));
            }
            if (userIdentity == null)
            {
                throw new ArgumentNullException(nameof(userIdentity));
            }
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }


            _objectBuilder = objectBuilder;
            _taskService = taskService;
            _historyLogService = historyLogService;
            _loggingManager = loggingManager;
            _userIdentity = userIdentity;
            _unitOfWork = unitOfWork;
        }

        public override void Execute(Task task, Guid? serviceInstanceId)
        {
            try
            {
                if (!Enum.IsDefined(typeof(ProvisionTaskType), task.Name))
                {
                    var message = string.Format(TaskResources.ProvisioningTaskRequest_InvalidRequestInvalidTaskName, task.Name);
                    throw new UnRecoverableErrorException(string.Format(message, task.Name));
                }

                // We do not want the task to retry if we have a problem
                var provisioningTask = _objectBuilder.Resolve<IProvisioningTaskProcessor>(task.Name);
                provisioningTask.Execute(task);

                // Complete the task
                _taskService.CompleteTask(task, true);
            }
            catch (UnRecoverableErrorException ex)
            {
                // Catch and log the exception
                _loggingManager.Write(LoggingEventSource.ProvisioningService, LoggingEventType.Error, ex.Message);

                // Complete UnRecoverable Exception
                _taskService.CompleteUnrecoverableTaskException(task, ex.Message);

            }
            catch (Exception ex)
            {
                // Catch and log the exception
                _loggingManager.Write(LoggingEventSource.ProvisioningService, LoggingEventType.Error, ex.Message);

                // Create a History Log
                var historyLog = task.CreateHistoryLog(_userIdentity.Name);
                historyLog.EventType = LoggingEventTypeNames.Error;
                historyLog.EventDetail = string.Format(TaskResources.UnexpectedApplicationError, "Provisioning");
                historyLog.Escalated = false;
                _historyLogService.Create(historyLog);

                // Complete the task
                _taskService.CompleteTask(task, false);
            }
        }
        public override void Dispose()
        {
            _unitOfWork?.Dispose();
        }
    }
}
