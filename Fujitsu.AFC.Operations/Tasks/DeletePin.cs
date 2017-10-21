using System;
using System.Diagnostics;
using System.Linq;
using Fujitsu.AFC.Core.Metrics;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Operations.Interfaces;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Tasks.Resources;
using Fujitsu.Exceptions.Framework;

namespace Fujitsu.AFC.Operations.Tasks
{
    public class DeletePin : IOperationsTaskProcessor
    {
        private readonly IPinService _pinService;
        private readonly ITaskService _taskService;
        private readonly IService<Site> _siteService;

        public DeletePin(IPinService pinService,
            ITaskService taskService,
            IService<Site> siteService)
        {
            if (pinService == null)
            {
                throw new ArgumentNullException(nameof(pinService));
            }
            if (taskService == null)
            {
                throw new ArgumentNullException(nameof(taskService));
            }
            if (siteService == null)
            {
                throw new ArgumentNullException(nameof(siteService));
            }

            _pinService = pinService;
            _taskService = taskService;
            _siteService = siteService;
        }

        public void Execute(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Operations.DeletePin.cs -> Processing Started.");
            var prfMonMethod = new PrfMon();

            if (task.Pin == null)
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoPin, task.Name);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (!_siteService.Query(x => x.Pin == task.Pin.Value).Any())
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_PinDoesNotExist, task.Name, task.Pin.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (_taskService.PendingMergeToPinOperation(task.Pin.Value, task.InsertedDate))
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_MergePinRequestPending, task.Name, task.Pin.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (_taskService.PendingMergeFromPinOperation(task.Pin.Value, task.InsertedDate))
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_MergePinRequestPending, task.Name, task.Pin.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            task.SiteId = _pinService.DeletePin(task);

            Debug.WriteLine("Fujitsu.AFC.Operations.DeletePin.cs -> Completed Processing - PIN: {0} Duration: {1:0.000}s", task.Pin.Value, prfMonMethod.Stop());

        }

    }
}
