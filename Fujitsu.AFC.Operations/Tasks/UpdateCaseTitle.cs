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
    public class UpdateCaseTitle : IOperationsTaskProcessor
    {
        private readonly ICaseService _caseService;
        private readonly ITaskService _taskService;
        private readonly ILibraryService _libraryService;
        private readonly IService<Site> _siteService;

        public UpdateCaseTitle(ICaseService caseService,
            ITaskService taskService,
            ILibraryService libraryService,
            IService<Site> siteService)
        {
            if (caseService == null)
            {
                throw new ArgumentNullException(nameof(caseService));
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

            _caseService = caseService;
            _taskService = taskService;
            _libraryService = libraryService;
            _siteService = siteService;
        }

        public void Execute(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Operations.UpdateCaseTitle.cs -> Processing Started.");
            var prfMonMethod = new PrfMon();

            if (task.Pin == null)
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoPin, task.Name);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (task.CaseId == null)
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoCaseId, task.Name, task.Pin.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (task.CaseTitle == null)
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoCaseTitle, task.Name, task.Pin.Value, task.CaseId.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (!_siteService.Query(x => x.Pin == task.Pin.Value).Any())
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_PinDoesNotExist, task.Name, task.Pin.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (!_libraryService.Query(x => x.CaseId == task.CaseId.Value).Any())
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_CaseIdDoesNotExist, task.Name, task.Pin.Value, task.CaseId.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (!_libraryService.Query(x => x.CaseId == task.CaseId.Value && x.Site.Pin == task.Pin.Value).Any())
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_PinDoesNotExistForTheSpecifiedCase, task.Name, task.Pin.Value, task.CaseId.Value);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            task.SiteId = _caseService.UpdateCaseTitle(task);

            Debug.WriteLine("Fujitsu.AFC.Operations.UpdateCaseTitle.cs -> Completed Processing - PIN: {0} CaseId: {1} CaseTitle {2} Duration: {3:0.000}s", task.Pin.Value, task.CaseId.Value, task.CaseTitle, prfMonMethod.Stop());
        }
    }
}
