using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.AFC.Data.Interfaces
{
    public interface IRepositoryTransaction : IDisposable
    {
        void Rollback();
        void Save();
    }
}
