using System;
using System.Diagnostics;
using Fujitsu.AFC.Core.Metrics;
using Fujitsu.AFC.Extensions;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Operations.Interfaces;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Tasks.Resources;
using Fujitsu.Exceptions.Framework;

namespace Fujitsu.AFC.Operations.Tasks
{
    public class UpdateCaseTitleByProject : IOperationsTaskProcessor
    {
        private readonly ICaseService _caseService;
        private readonly ITaskService _taskService;

        public UpdateCaseTitleByProject(ICaseService caseService,
                ITaskService taskService)
        {
            if (caseService == null)
            {
                throw new ArgumentNullException(nameof(caseService));
            }
            if (taskService == null)
            {
                throw new ArgumentNullException(nameof(taskService));
            }

            _caseService = caseService;
            _taskService = taskService;
        }

        public void Execute(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Operations.UpdateCaseTitleByProject.cs -> Processing Started.");
            var prfMonMethod = new PrfMon();

            if (task.ProjectId == null)
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoProjectIdWithoutPin, task.Name);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            if (string.IsNullOrEmpty(task.ProjectName))
            {
                var message = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoProjectName, task.Name, task.ProjectId);
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
                var message = string.Format(TaskResources.OperationsTaskRequest_InvalidDictionaryXMLWithoutPin, task.Name, task.ProjectId);
                _taskService.CompleteUnrecoverableTaskException(task, message);
                throw new UnRecoverableErrorException(message);
            }

            _caseService.UpdateCaseTitleByProject(task);

            Debug.WriteLine("Fujitsu.AFC.Operations.UpdateCaseTitleByProject.cs -> Completed Processing - ProjectId: {0} ProjectName: {1} Duration: {2:0.000}s", task.ProjectId.Value, task.ProjectName, prfMonMethod.Stop());

        }
    }
}
