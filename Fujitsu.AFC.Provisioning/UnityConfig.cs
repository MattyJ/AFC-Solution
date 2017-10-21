using System;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Provisioning.Interfaces;
using Fujitsu.AFC.Provisioning.Tasks;
using Microsoft.Practices.Unity;

namespace Fujitsu.AFC.Provisioning
{
    public class UnityConfig
    {
        public static void RegisterTypes(IUnityContainer container, Func<LifetimeManager> manager)
        {
            container.RegisterType<IProvisioningTaskProcessor, CaseSiteProvisioning>(TaskNames.CaseSiteProvisioning, manager());
        }
    }
}
