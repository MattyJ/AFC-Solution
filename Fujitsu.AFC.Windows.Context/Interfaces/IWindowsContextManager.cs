using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.AFC.Windows.Context.Interfaces
{
    public interface IWindowsContextManager
    {
        IWindowsUserManager UserManager { get; }
    }
}
