using Fujitsu.AFC.Support.WindowsService.Interface;
using Microsoft.Practices.Unity;

namespace Fujitsu.AFC.Support.WindowsService
{
    public class UnityConfig
    {
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<ISupportProcessor, SupportProcessor>();
        }
    }
}
