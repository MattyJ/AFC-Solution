using System.Security.Principal;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Windows.Context.Interfaces;

namespace Fujitsu.AFC.Windows.Context
{
    public class WindowsUserManager : IWindowsUserManager, IUserIdentity
    {
        private readonly WindowsIdentity _windowsIdentity;

        public WindowsUserManager() : this(WindowsIdentity.GetCurrent())
        {
        }

        public WindowsUserManager(WindowsIdentity windowsIdentity)
        {
            _windowsIdentity = windowsIdentity;
        }

        public string Name => _windowsIdentity.Name;

        public string UserName { get; private set; }
    }
}
