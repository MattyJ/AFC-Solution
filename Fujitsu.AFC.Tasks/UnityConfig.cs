using System;
using System.Diagnostics.CodeAnalysis;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Caching;
using Fujitsu.AFC.Core.Injection;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Core.Log;
using Fujitsu.AFC.Data;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Data.Repository;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Operations.Handler;
using Fujitsu.AFC.Provisioning.Handler;
using Fujitsu.AFC.Services;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Support.Handler;
using Fujitsu.AFC.Tasks.Interfaces;
using Fujitsu.AFC.Tasks.Managers;
using Fujitsu.AFC.Windows.Context;
using Microsoft.Practices.Unity;

namespace Fujitsu.AFC.Tasks
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UnityConfig
    {

        public static void RegisterTypes(IUnityContainer container)
        {
            Func<LifetimeManager> contextManager = () => new HierarchicalLifetimeManager();
            Func<LifetimeManager> manager = () => new HierarchicalLifetimeManager();

            #region Data Context

            const string connectionString = "Name=AFCDataContext";
            var injection = new InjectionConstructor(connectionString);

            container.RegisterType<IAFCDataContext, AFCDataContext>(contextManager(), injection);
            container.RegisterType<AFCDataContext, AFCDataContext>(contextManager(), injection);
            container.RegisterType<IUnitOfWork, UnitOfWork>(manager());

            #endregion

            #region Core

            container.RegisterType<ILoggingManager, LoggingManager>(manager());
            container.RegisterType<ICacheManager, CacheManager>(manager());

            #endregion

            #region Repositories

            container.RegisterType<IRepository<Task>, Repository<Task>>(manager());
            container.RegisterType<IRepository<HistoryLog>, Repository<HistoryLog>>(manager());
            container.RegisterType<IRepository<TimerLock>, Repository<TimerLock>>(manager());
            container.RegisterType<IRepository<ProvisionedSiteCollection>, Repository<ProvisionedSiteCollection>>(manager());
            container.RegisterType<IRepository<ProvisionedSite>, Repository<ProvisionedSite>>(manager());
            container.RegisterType<IRepository<Site>, Repository<Site>>(manager());
            container.RegisterType<IRepository<Library>, Repository<Library>>(manager());
            container.RegisterType<IRepository<Parameter>, Repository<Parameter>>(manager());

            #endregion

            #region Services

            container.RegisterType<IService<HistoryLog>, HistoryLogService>(manager());
            container.RegisterType<IParameterService, ParameterService>(manager());
            container.RegisterType<ITaskService, TaskService>(manager());
            container.RegisterType<ITimerLockService, TimerLockService>(manager());
            container.RegisterType<IProvisioningService, ProvisioningService>(manager());
            container.RegisterType<IService<Site>, SiteService>(manager());
            container.RegisterType<IPinService, PinService>(manager());
            container.RegisterType<ICaseService, CaseService>(manager());
            container.RegisterType<ILibraryService, LibraryService>(manager());
            container.RegisterType<ISupportService, SupportService>(manager());
            container.RegisterType<IUserService, UserService>(manager());
            container.RegisterType<IContextService, ContextService>(manager());

            #endregion

            #region Task Handler Managers

            container.RegisterType<ITaskHandlerManager, ProvisioningHandlerManager>(TaskHandlerNames.ProvisioningHandler, manager());
            container.RegisterType<ITaskHandlerManager, OperationsHandlerManager>(TaskHandlerNames.OperationsHandler, manager());
            container.RegisterType<ITaskHandlerManager, SupportHandlerManager>(TaskHandlerNames.SupportHandler, manager());

            #endregion

            #region Task Handlers

            container.RegisterType<ITaskHandler, ProvisioningHandler>(TaskHandlerNames.ProvisioningHandler, manager());
            container.RegisterType<ITaskHandler, OperationsHandler>(TaskHandlerNames.OperationsHandler, manager());
            container.RegisterType<ITaskHandler, SupportHandler>(TaskHandlerNames.SupportHandler, manager());

            #endregion

            #region Special

            container.RegisterType<IObjectBuilder, ObjectBuilder>(new ContainerControlledLifetimeManager());
            container.RegisterType<IUserIdentity, WindowsUserManager>(manager(), new InjectionConstructor());

            #endregion

            #region External Unity Configurations

            Fujitsu.AFC.Provisioning.UnityConfig.RegisterTypes(container, manager);
            Fujitsu.AFC.Operations.UnityConfig.RegisterTypes(container, manager);
            Fujitsu.AFC.Support.UnityConfig.RegisterTypes(container, manager);
            Fujitsu.Exceptions.Framework.UnityConfig.RegisterTypes(container, manager);

            #endregion
        }
    }

}
