using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Core.Log;
using Fujitsu.AFC.Enumerations;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.AFC.Core.Tests
{
    [TestClass]
    public class LoggingManagerTests
    {
        private ILoggingManager _target;

        [TestInitialize]
        public void Initialize()
        {
            // Set the LogWriter for the static Logger class
            Logger.SetLogWriter(new LogWriterFactory().Create());

            _target = new LoggingManager();
        }

        [TestMethod]
        public void LoggingManager_Execute_LoggingAllEventTypeError_ExecutesSuccessfully()
        {
            _target.Write(LoggingEventSource.ProvisioningService, LoggingEventType.Error, "Test Message");
            _target.Write(LoggingEventSource.ProvisioningService, LoggingEventType.Warning, "Test Message");
            _target.Write(LoggingEventSource.ProvisioningService, LoggingEventType.Information, "Test Message");
        }
    }
}
