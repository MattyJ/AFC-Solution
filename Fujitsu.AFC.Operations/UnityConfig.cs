using System;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Operations.Interfaces;
using Fujitsu.AFC.Operations.Tasks;
using Microsoft.Practices.Unity;

namespace Fujitsu.AFC.Operations
{
    public class UnityConfig
    {
        public static void RegisterTypes(IUnityContainer container, Func<LifetimeManager> manager)
        {
            container.RegisterType<IOperationsTaskProcessor, AllocatePin>(TaskNames.AllocatePin, manager());
            container.RegisterType<IOperationsTaskProcessor, UpdateServiceUserTitle>(TaskNames.UpdateServiceUserTitle, manager());
            container.RegisterType<IOperationsTaskProcessor, DeletePin>(TaskNames.DeletePin, manager());
            container.RegisterType<IOperationsTaskProcessor, UpdatePinWithDictionaryValues>(TaskNames.UpdatePinWithDictionaryValues, manager());
            container.RegisterType<IOperationsTaskProcessor, MergePin>(TaskNames.MergePin, manager());

            container.RegisterType<IOperationsTaskProcessor, AllocateCase>(TaskNames.AllocateCase, manager());
            container.RegisterType<IOperationsTaskProcessor, UpdateCaseTitle>(TaskNames.UpdateCaseTitle, manager());
            container.RegisterType<IOperationsTaskProcessor, UpdateCaseTitleByProject>(TaskNames.UpdateCaseTitleByProject, manager());
            container.RegisterType<IOperationsTaskProcessor, UpdateCaseWithDictionaryValues>(TaskNames.UpdateCaseWithDictionaryValues, manager());
            container.RegisterType<IOperationsTaskProcessor, ArchiveCase>(TaskNames.ArchiveCase, manager());
            container.RegisterType<IOperationsTaskProcessor, ChangePrimaryProject>(TaskNames.ChangePrimaryProject, manager());
            container.RegisterType<IOperationsTaskProcessor, MoveCase>(TaskNames.MoveCase, manager());
            container.RegisterType<IOperationsTaskProcessor, CloseCase>(TaskNames.CloseCase, manager());
            container.RegisterType<IOperationsTaskProcessor, DeleteCase>(TaskNames.DeleteCase, manager());

            container.RegisterType<IOperationsTaskProcessor, RestrictUser>(TaskNames.RestrictUser, manager());
            container.RegisterType<IOperationsTaskProcessor, RemoveRestrictedUser>(TaskNames.RemoveRestrictedUser, manager());
        }
    }
}
