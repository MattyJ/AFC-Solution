using System.Collections.Generic;
using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Services.Interfaces
{
    public interface ILibraryService : IService<Library>
    {
        Dictionary<string, List<Library>> GetSiteCollectionLibraryDictionary(TaskEntity task);
    }
}
