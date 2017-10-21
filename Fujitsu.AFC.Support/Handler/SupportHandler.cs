using System;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Handlers;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Enumerations;
using Fujitsu.AFC.Extensions;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Support.Interfaces;
using Fujitsu.AFC.Tasks.Resources;
using Fujitsu.Exceptions.Framework;

namespace Fujitsu.AFC.Support.Handler
{
    public class SupportHandler : TaskHandler
    {
        private readonly IObjectBuilder _objectBuilder;
        private readonly ITaskService _taskService;
        private readonly IService<HistoryLog> _historyLogService;
        private readonly ILoggingManager _loggingManager;
        private readonly IUserIdentity _userIdentity;
        private readonly IUnitOfWork _unitOfWork;

        public SupportHandler(IObjectBuilder objectBuilder,
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
                if (!Enum.IsDefined(typeof(SupportTaskType), task.Name))
                {
                    var message = string.Format(TaskResources.SupportTaskRequest_InvalidRequestInvalidTaskName, task.Name);
                    throw new UnRecoverableErrorException(string.Format(message, task.Name));
                }

                // Execute the support task
                var supportTask = _objectBuilder.Resolve<ISupportTaskProcessor>(task.Name);
                supportTask.Execute(task);

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
                historyLog.EventDetail = string.Format(TaskResources.UnexpectedApplicationError, "Support");
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
