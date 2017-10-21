using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using Fujitsu.AFC.Core.Injection;
using Fujitsu.AFC.Core.Interfaces;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.Exceptions.Framework.Tests.ExceptionFormatters
{
    [TestClass]
    public class DbEntityValidationExceptionFormatterTests
    {
        private IObjectBuilder _objectBuilder;
        private DbEntityValidationException _dbEntityValidationException;

        [TestInitialize]
        public void TestIntialize()
        {
            _objectBuilder = new ObjectBuilder(SetupObjectBuilder);

            _dbEntityValidationException = new DbEntityValidationException("Foobar", new List<DbEntityValidationResult>());
        }

        [TestMethod]
        public void DbEntityValidationExceptionFormatter_ToString_ExceptionMessage_MessageAndStackTraceReturned()
        {
            var message = string.Empty;
            try
            {
                throw _dbEntityValidationException;
            }
            catch (Exception ex)
            {
                message = ex.Flatten();
            }
            Assert.IsTrue(message.Contains("Foobar"));
            Assert.IsTrue(message.Contains("DbEntityValidationExceptionFormatterTests"));
        }

        private static void SetupObjectBuilder(IUnityContainer container)
        {
            Fujitsu.Exceptions.Framework.UnityConfig.RegisterTypes(container, () => new HierarchicalLifetimeManager());
        }
    }
}