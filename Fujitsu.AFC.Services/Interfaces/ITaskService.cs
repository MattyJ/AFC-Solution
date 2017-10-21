using System;
using System.Collections.Generic;
using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Services.Interfaces
{
    public interface ITaskService : IService<Task>
    {
        IEnumerable<Task> AllProvisioningHandlerTasks();

        IEnumerable<Task> AllOperationsHandlerTasks();

        IEnumerable<Task> AllSupportHandlerTasks();

        void UpdateTask(Task task);

        void CompleteUnrecoverableTaskException(Task task, string message);

        void CompleteTask(Task task, bool success);

        bool PendingAllocatePinOperation(int pin, DateTime insertedDate);

        bool PendingMergeFromPinOperation(int pin, DateTime insertedDate);

        bool PendingMergeToPinOperation(int pin, DateTime insertedDate);
    }
}
