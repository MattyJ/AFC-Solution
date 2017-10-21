using Fujitsu.AFC.Operations.WindowsService.Interface;
using Microsoft.Practices.Unity;

namespace Fujitsu.AFC.Operations.WindowsService
{
    public class UnityConfig
    {
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IOperationsProcessor, OperationsProcessor>();
        }
    }
}
