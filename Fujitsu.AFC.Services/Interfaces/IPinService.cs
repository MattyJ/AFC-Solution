using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Services.Interfaces
{
    public interface IPinService
    {
        int AllocatePin(Task task);
        int DeletePin(Task task);
        void MergePin(Task task);
        int UpdatePinTitle(Task task);
        int ChangePrimaryProject(Task task);
        int UpdatePinWithDictionaryValues(Task task);
    }
}
