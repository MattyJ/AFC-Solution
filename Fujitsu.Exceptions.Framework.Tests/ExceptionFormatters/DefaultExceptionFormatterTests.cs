using System;
using Fujitsu.AFC.Core.Injection;
using Fujitsu.AFC.Core.Interfaces;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.Exceptions.Framework.Tests.ExceptionFormatters
{
    [TestClass]
    public class DefaultExceptionFormatterTests
    {
        private IObjectBuilder _objectBuilder;

        [TestInitialize]
        public void TestIntialize()
        {
            _objectBuilder = new ObjectBuilder(SetupObjectBuilder);
        }

        [TestMethod]
        public void DefaultExceptionFormatter_ToString_ExceptionMessage_MessageAndStackTraceReturned()
        {
            string message;
            try
            {
                throw new Exception("Foobar");
            }
            catch (Exception ex)
            {
                message = ex.Flatten();
            }
            Assert.IsTrue(message.Contains("Foobar"));
            Assert.IsTrue(message.Contains("DefaultExceptionFormatterTests"));
        }

        [TestMethod]
        public void DefaultExceptionFormatter_ToString_ExceptionMessage_InnerExceptionProcessed()
        {
            string message;
            try
            {
                throw new Exception("Foobar", new Exception("InnerException"));
            }
            catch (Exception ex)
            {
                message = ex.Flatten();
            }
            Assert.IsTrue(message.Contains("Foobar"));
            Assert.IsTrue(message.Contains("InnerException"));
            Assert.IsTrue(message.Contains("DefaultExceptionFormatterTests"));
        }

        private static void SetupObjectBuilder(IUnityContainer container)
        {
            Fujitsu.Exceptions.Framework.UnityConfig.RegisterTypes(container, () => new HierarchicalLifetimeManager());
        }
    }
}