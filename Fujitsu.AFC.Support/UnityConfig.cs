using System;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Support.Interfaces;
using Fujitsu.AFC.Support.Tasks;
using Microsoft.Practices.Unity;

namespace Fujitsu.AFC.Support
{
    public class UnityConfig
    {
        public static void RegisterTypes(IUnityContainer container, Func<LifetimeManager> manager)
        {
            container.RegisterType<ISupportTaskProcessor, HistoryErrorLogMonitoring>(TaskNames.HistoryErrorLogMonitoring, manager());
        }
    }
}
