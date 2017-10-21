using System;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Extensions.CSOM;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Common;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.Exceptions.Framework;
using Microsoft.SharePoint.Client;

namespace Fujitsu.AFC.Services
{
    public class SupportService : SharePointOnline, ISupportService
    {
        private readonly IRepository<HistoryLog> _historyLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserIdentity _userIdentity;

        public SupportService(IParameterService parameterService, IRepository<HistoryLog> historyLogRepository, IUserIdentity userIdentity, IUnitOfWork unitOfWork) : base(parameterService)
        {
            if (historyLogRepository == null)
            {
                throw new ArgumentNullException(nameof(historyLogRepository));
            }
            if (userIdentity == null)
            {
                throw new ArgumentNullException(nameof(userIdentity));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _historyLogRepository = historyLogRepository;
            _userIdentity = userIdentity;
            _unitOfWork = unitOfWork;
        }
        public void EscalateErrorEvent(HistoryLog historyLog)
        {
            using (var context = new ClientContext(SupportSiteCollectionUrl()))
            {
                // Assign Credentials
                context.Credentials = Credentials;

                context.Web.AddEscalationListItem(SupportEscalationListName, historyLog);
                context.ExecuteQuery();
            }

            RetryableOperation.Invoke(ExceptionPolicies.General,
             () =>
             {
                 // Mark the dead task as escalated
                 historyLog.Escalated = true;
                 historyLog.UpdatedDate = DateTime.Now;
                 historyLog.UpdatedBy = _userIdentity.Name;
                 _historyLogRepository.Update(historyLog);
                 _unitOfWork.Save();
             });
        }
    }
}
