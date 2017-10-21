using Fujitsu.AFC.Provisioning.WindowsService.Interface;
using Microsoft.Practices.Unity;

namespace Fujitsu.AFC.Provisioning.WindowsService
{
    public class UnityConfig
    {
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IProvisioningProcessor, ProvisioningProcessor>();
        }
    }
}
