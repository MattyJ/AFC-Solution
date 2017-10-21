using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Services.Interfaces
{
    public interface ICaseService
    {
        int AllocateCase(Task task);
        int UpdateCaseTitle(Task task);
        int MoveCase(Task task);
        int CloseCase(Task task);
        int ArchiveCase(Task task);
        void DeleteCase(Task task);
        int UpdateCaseWithDictionaryValues(Task task);
        void UpdateCaseTitleByProject(Task task);
    }
}
