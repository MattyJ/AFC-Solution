using System;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Data.Handlers
{
    public abstract class TaskHandler : ITaskHandler
    {
        public abstract void Execute(Task task, Guid? serviceInstanceId);

        public virtual bool CanExecute(Task task, Guid? serverInstanceId)
        {
            return DateTime.Compare(DateTime.Now, task.NextScheduledDate.GetValueOrDefault()) > 0;
        }

        public abstract void Dispose();
    }
}
