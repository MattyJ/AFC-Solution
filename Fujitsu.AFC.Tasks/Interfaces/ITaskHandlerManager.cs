using System;

namespace Fujitsu.AFC.Tasks.Interfaces
{
    public interface ITaskHandlerManager
    {
        void Execute();

        bool CanExecute(int hour);
    }
}
