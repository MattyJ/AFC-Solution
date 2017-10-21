using System;
using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Services.Interfaces
{
    public interface ITimerLockService : IService<TimerLock>
    {
        void AcquireLock(Guid serviceInstanceId, int pinId, int taskId);
    }
}
