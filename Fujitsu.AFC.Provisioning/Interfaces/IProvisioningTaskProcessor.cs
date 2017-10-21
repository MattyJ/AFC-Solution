using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Provisioning.Interfaces
{
    public interface IProvisioningTaskProcessor
    {
        void Execute(Task task);
    }
}