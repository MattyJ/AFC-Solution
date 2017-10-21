using Fujitsu.AFC.Core.Metrics;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Operations.Interfaces;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Tasks.Resources;
using Fujitsu.Exceptions.Framework;
using System;
using System.Diagnostics;
using System.Linq;
using Task = Fujitsu.AFC.Model.Task;

namespace Fujitsu.AFC.Operations.Tasks
{
    public class HelloWorldPin : IOperationsTaskProcessor
    {
        private readonly IPinService _pinService;
        private readonly ITaskService _taskService;
        private readonly IService<Site> _siteService;

        public HelloWorldPin(IPinService pinService,
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
            Debug.WriteLine("Fujitsu.AFC.Operations.HelloWorldPin.cs -> Processing Started.");
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

            task.SiteId = _pinService.HelloWorldPin(task);
            Debug.WriteLine("Fujitsu.AFC.Operations.HelloWorldPin.cs -> Completed Processing - PIN: {0} Title {1} Duration: {2:0.000}s", task.Pin.Value, task.SiteTitle, prfMonMethod.Stop());
        }
    }
}
