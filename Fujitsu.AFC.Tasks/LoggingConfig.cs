using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.Unity;

namespace Fujitsu.AFC.Tasks
{
    public static class LoggingConfig
    {
        public static void Initialise(IUnityContainer container)
        {
            Logger.SetLogWriter(new LogWriterFactory().Create());

            var config = ConfigurationSourceFactory.Create();
            var factory = new ExceptionPolicyFactory(config);

            var exceptionManager = factory.CreateManager();
            container.RegisterInstance(exceptionManager);

            ExceptionPolicy.SetExceptionManager(exceptionManager);
        }
    }
}
