using System;
using System.Diagnostics;
using System.Linq;
using Fujitsu.AFC.Core.Metrics;
using Fujitsu.AFC.Extensions;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Operations.Interfaces;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Tasks.Resources;
using Fujitsu.Exceptions.Framework;

namespace Fujitsu.AFC.Operations.Tasks
{
    public class AllocatePin : IOperationsTaskProcessor
    {
        private readonly IProvisioningService _provisioningService;
        private readonly IPinService _pinService;
        private readonly ITaskService _taskService;
        private readonly IService<Site> _siteService;

        public AllocatePin(IProvisioningService provisioningService,
            IPinService pinService,
            ITaskService taskService,
            IService<Site> siteService)
        {
            if (provisioningService == null)
            {
                throw new ArgumentNullException(nameof(provisioningService));
            }
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

            _provisioningService = provisioningService;
            _pinService = pinService;
            _taskService = taskService;
            _siteService = siteService;
        }

        public void Execute(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Operations.AllocatePin.cs -> Processing Started.");
            var prfMonMethod = new PrfMon();

            if (task.Pin == null)
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoPin, task.Name);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (task.SiteTitle.IsNullOrEmpty())
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoSiteTitle, task.Name, task.Pin.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (string.IsNullOrEmpty(task.Dictionary))
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoDictionaryXML, task.Name);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (!task.Dictionary.IsValidXml())
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_InvalidDictionaryXML, task.Name, task.Pin.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (_provisioningService.GetNumberOfUnallocatedSites() == 0)
            {
                throw new ApplicationException(string.Format(TaskResources.OperationsTaskRequest_NoUnallocatedSitesAvailable, task.Name, task.Pin.Value));
            }

            if (_siteService.Query(x => x.Pin == task.Pin.Value).Any())
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_PinAlreadyExists, task.Name, task.Pin.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (_taskService.PendingAllocatePinOperation(task.Pin.Value, task.InsertedDate))
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_PinRequestPending, task.Name, task.Pin.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            task.SiteId = _pinService.AllocatePin(task);

            Debug.WriteLine("Fujitsu.AFC.Operations.AllocatePin.cs -> Completed Processing - PIN: {0} SiteTitle: {1} Duration: {2:0.000}s", task.Pin.Value, task.SiteTitle, prfMonMethod.Stop());
        }
    }
}
