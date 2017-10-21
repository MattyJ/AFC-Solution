using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Services.Interfaces
{
    public interface IUserService
    {
        void RestictUser(Task task);
        void RemoveRestrictedUser(Task task);
    }
}
