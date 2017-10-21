using System;
using System.Security.Principal;
using Fujitsu.AFC.Windows.Context.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.AFC.Windows.Context.Tests
{
    [TestClass]
    public class WindowsContextManagerTests
    {
        private IWindowsContextManager _contextManager;

        [TestInitialize]
        public void Setup()
        {
            _contextManager = WindowsContextManager.GetContextManager(WindowsIdentity.GetCurrent());
        }

        [TestMethod]
        public void WindowsContextManager_UserManager_ReturnsUserManagerInstance()
        {
            var userManager = _contextManager.UserManager;
            Assert.IsNotNull(userManager);
            Assert.IsInstanceOfType(userManager, typeof(IWindowsUserManager));
        }
    }
}
