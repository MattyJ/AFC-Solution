using System;
using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Data.Interfaces
{
    public interface ITaskHandler : IDisposable
    {
        void Execute(Task task, Guid? serverInstanceId);
        bool CanExecute(Task task, Guid? serverInstanceId);
    }
}
