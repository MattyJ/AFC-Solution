using Fujitsu.AFC.Core.Metrics;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Operations.Interfaces;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Tasks.Resources;
using Fujitsu.Exceptions.Framework;
using System;
using System.Diagnostics;
using System.Linq;

namespace Fujitsu.AFC.Operations.Tasks
{
    public class MergePin : IOperationsTaskProcessor
    {
        private readonly IPinService _pinService;
        private readonly ITaskService _taskService;
        private readonly IService<Site> _siteService;

        public MergePin(IPinService pinService,
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
            Debug.WriteLine("Fujitsu.AFC.Operations.MergePin.cs -> Processing Started.");
            var prfMonMethod = new PrfMon();

            if (task.ToPin == null)
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoPin, task.Name);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (task.FromPin == null)
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoPin, task.Name);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (!_siteService.Query(x => x.Pin == task.ToPin.Value).Any())
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_PinDoesNotExist, task.Name, task.ToPin.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (!_siteService.Query(x => x.Pin == task.FromPin.Value).Any())
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_PinDoesNotExist, task.Name, task.FromPin.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            _pinService.MergePin(task);

            Debug.WriteLine("Fujitsu.AFC.Operations.MergePin.cs -> Completed Processing - FromPIN: {0} ToPIN : {1} Duration: {2:0.000}s", task.FromPin.Value, task.ToPin.Value, prfMonMethod.Stop());

        }
    }
}
