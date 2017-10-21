using Fujitsu.AFC.Core.Metrics;
using Fujitsu.AFC.Extensions;
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
    public class ChangePrimaryProject : IOperationsTaskProcessor
    {
        private readonly IPinService _pinService;
        private readonly ITaskService _taskService;
        private readonly ILibraryService _libraryService;
        private readonly IService<Site> _siteService;

        public ChangePrimaryProject(IPinService pinService,
            ITaskService taskService,
            ILibraryService libraryService,
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
            if (libraryService == null)
            {
                throw new ArgumentNullException(nameof(libraryService));
            }
            if (siteService == null)
            {
                throw new ArgumentNullException(nameof(siteService));
            }

            _pinService = pinService;
            _taskService = taskService;
            _libraryService = libraryService;
            _siteService = siteService;
        }

        public void Execute(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Operations.ChangePrimaryProject.cs -> Processing Started.");
            var prfMonMethod = new PrfMon();

            if (task.Pin == null)
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoPin, task.Name);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (task.CurrentProjectId == null)
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoCurrentProjectId, task.Name, task.Pin.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (task.NewProjectId == null)
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoNewProjectId, task.Name, task.Pin.Value);
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

            if (!_siteService.Query(x => x.Pin == task.Pin.Value).Any())
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_PinDoesNotExist, task.Name, task.Pin.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (!_libraryService.Query(x => x.ProjectId == task.CurrentProjectId.Value).Any())
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_CaseDoesNotExistForSpecifiedProjectId, task.Name, task.CurrentProjectId.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (!_libraryService.Query(x => x.ProjectId == task.NewProjectId.Value).Any())
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_CaseDoesNotExistForSpecifiedProjectId, task.Name, task.NewProjectId.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (!_libraryService.Query(x => x.ProjectId == task.CurrentProjectId.Value && x.Site.Pin == task.Pin.Value).Any())
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_CaseDoesNotExistForTheSpecifiedProjectIdOnThePin, task.Name, task.Pin.Value, task.CurrentProjectId.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (!_libraryService.Query(x => x.ProjectId == task.NewProjectId.Value && x.Site.Pin == task.Pin.Value).Any())
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_CaseDoesNotExistForTheSpecifiedProjectIdOnThePin, task.Name, task.Pin.Value, task.NewProjectId.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            task.SiteId = _pinService.ChangePrimaryProject(task);

            Debug.WriteLine("Fujitsu.AFC.Operations.ChangePrimaryProject.cs -> Completed Processing - PIN: {0} CurrentProjectId: {1} NewProjectId {2} Duration: {3:0.000}s", task.Pin.Value, task.CurrentProjectId.Value, task.NewProjectId.Value, prfMonMethod.Stop());
        }
    }
}
