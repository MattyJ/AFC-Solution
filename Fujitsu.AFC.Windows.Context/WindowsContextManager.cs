using System.Security.Principal;
using Fujitsu.AFC.Windows.Context.Interfaces;

namespace Fujitsu.AFC.Windows.Context
{
    public class WindowsContextManager : IWindowsContextManager
    {
        public IWindowsUserManager UserManager { get; private set; }

        public WindowsContextManager() : this(WindowsIdentity.GetCurrent())
        {
        }

        private WindowsContextManager(WindowsIdentity windowsIdentity)
        {
            UserManager = new WindowsUserManager(windowsIdentity);
        }

        public static IWindowsContextManager GetContextManager(WindowsIdentity windowsIdentity)
        {
            return new WindowsContextManager(windowsIdentity);
        }
    }
}
