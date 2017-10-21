using System.Security.Principal;
using Fujitsu.AFC.Windows.Context.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.AFC.Windows.Context.Tests
{
    [TestClass]
    public class WindowsUserManagerTests
    {
        private IWindowsUserManager _userManager;
        private WindowsIdentity _windowsIdentity;

        [TestInitialize]
        public void Setup()
        {
            _windowsIdentity = WindowsIdentity.GetCurrent();
            _userManager = new WindowsUserManager(_windowsIdentity);
        }

        [TestMethod]
        public void WindowsUserManager_Name_ReturnsCorrectName()
        {
            var userName = _userManager.Name;
            Assert.IsNotNull(userName);
            Assert.AreEqual(_windowsIdentity.Name, userName);
        }
    }
}
