using System;
using System.Linq;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Enumerations;
using Fujitsu.AFC.Services.Interfaces;

namespace Fujitsu.AFC.Tasks.Managers
{
    public class SupportHandlerManager : TaskHandlerManager
    {
        private readonly ITaskService _taskService;
        private readonly IObjectBuilder _objectBuilder;
        private readonly ILoggingManager _loggingManager;

        public SupportHandlerManager(ITaskService taskService,
            IObjectBuilder objectBuilder,
            ILoggingManager loggingManager)
        {
            if (taskService == null)
            {
                throw new ArgumentNullException(nameof(taskService));
            }
            if (objectBuilder == null)
            {
                throw new ArgumentNullException(nameof(objectBuilder));
            }
            if (loggingManager == null)
            {
                throw new ArgumentNullException(nameof(loggingManager));
            }

            _taskService = taskService;
            _objectBuilder = objectBuilder;
            _loggingManager = loggingManager;
        }

        public override void Execute()
        {
            try
            {
                // Get all outstanding support tasks
                var tasks = _taskService.AllSupportHandlerTasks().ToList();

                if (tasks.Any())
                {
                    foreach (var task in tasks)
                    {
                        try
                        {
                            // Create a new DI container to keep all work with the handler discrete
                            _objectBuilder.AddChildContainer();

                            // Instantiate the task handler
                            using (var taskHandler = _objectBuilder.Resolve<ITaskHandler>(task.Handler))
                            {

                                // Check that the task is scheduled to be executed
                                if (!taskHandler.CanExecute(task, null))
                                {
                                    return;
                                }

                                taskHandler.Execute(task, null);
                            }
                        }
                        finally
                        {
                            _objectBuilder.RemoveAllChildContainers();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _loggingManager.Write(LoggingEventSource.SupportService, LoggingEventType.Error, ex.Message);
            }
        }
    }
}
