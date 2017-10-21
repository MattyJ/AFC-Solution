using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Handlers;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Enumerations;
using Fujitsu.AFC.Extensions;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Operations.Interfaces;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Tasks.Resources;
using Fujitsu.Exceptions.Framework;
using System;
using System.Linq;

namespace Fujitsu.AFC.Operations.Handler
{
    public class OperationsHandler : TaskHandler
    {
        private readonly IObjectBuilder _objectBuilder;
        private readonly ITaskService _taskService;
        private readonly ITimerLockService _timerLockService;
        private readonly IUnitOfWork _unitOfWork;

        public OperationsHandler(IObjectBuilder objectBuilder,
            ITimerLockService timerLockService,
            ITaskService taskService,
            IUnitOfWork unitOfWork)
        {
            if (objectBuilder == null)
            {
                throw new ArgumentNullException(nameof(objectBuilder));
            }
            if (timerLockService == null)
            {
                throw new ArgumentNullException(nameof(timerLockService));
            }
            if (taskService == null)
            {
                throw new ArgumentNullException(nameof(taskService));
            }
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _objectBuilder = objectBuilder;
            _taskService = taskService;
            _timerLockService = timerLockService;
            _unitOfWork = unitOfWork;
        }

        public override void Execute(Task task, Guid? serviceInstanceId)
        {
            if (!Enum.IsDefined(typeof(OperationTaskType), task.Name))
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestInvalidTaskName, task.Name);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            // Acquire the lock
            _timerLockService.AcquireLock(serviceInstanceId.Value, task.Pin ?? 0, task.Id);

            if (task.Name.SafeEquals(TaskNames.MergePin))
            {
                _timerLockService.AcquireLock(serviceInstanceId.Value, task.FromPin ?? 0, task.Id);
                _timerLockService.AcquireLock(serviceInstanceId.Value, task.ToPin ?? 0, task.Id);
            }

            // Execute the operation task
            var operationsTask = _objectBuilder.Resolve<IOperationsTaskProcessor>(task.Name);
            operationsTask.Execute(task);

            // Complete the task
            _taskService.CompleteTask(task, true);
        }


        public override bool CanExecute(Task task, Guid? serverInstanceId)
        {
            if (serverInstanceId != null && task.Pin != null)
            {
                var timerLocks = _timerLockService.All().ToList();

                if (timerLocks.Any())
                {
                    if (timerLocks.Any(x => x.LockedInstance != serverInstanceId.Value && x.LockedPin == task.Pin.Value))
                    {
                        // Only the locked server instance id can process tasks for this Pin whilst locked
                        return false;
                    }

                    if (timerLocks.Any(x => x.LockedInstance == serverInstanceId.Value && x.LockedPin == task.Pin.Value && x.TaskId != task.Id))
                    {
                        // Once locked the server instance id cannot complete any other the request for that Pin until the locked Task is completed
                        return false;
                    }
                }

            }

            return DateTime.Compare(DateTime.Now, task.NextScheduledDate.GetValueOrDefault()) > 0;
        }
        public override void Dispose()
        {
            _unitOfWork?.Dispose();
        }
    }
}
