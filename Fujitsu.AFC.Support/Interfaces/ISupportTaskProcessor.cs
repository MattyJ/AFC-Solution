using Fujitsu.AFC.Data.Handlers;
using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Support.Interfaces
{
    public interface ISupportTaskProcessor
    {
        void Execute(Task task);
    }
}
