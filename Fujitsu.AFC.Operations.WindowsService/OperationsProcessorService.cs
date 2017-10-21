using System.Diagnostics.CodeAnalysis;
using System.ServiceProcess;
using Fujitsu.AFC.Core.Injection;
using Fujitsu.AFC.Operations.WindowsService.Interface;
using Fujitsu.AFC.Tasks;

namespace Fujitsu.AFC.Operations.WindowsService
{
    [ExcludeFromCodeCoverage]
    static class OperationsProcessorService
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            // Setup the framework.
            Bootstrapper.Initialise();

            //System.Diagnostics.Debugger.Launch();

            var objectBuilder = new ObjectBuilder(UnityConfig.RegisterTypes, Fujitsu.AFC.Tasks.UnityConfig.RegisterTypes);
            var service = objectBuilder.Resolve<IOperationsProcessor>() as ServiceBase;

            var servicesToRun = new ServiceBase[]
            {
                service
            };

            ServiceBase.Run(servicesToRun);
        }
    }
}
