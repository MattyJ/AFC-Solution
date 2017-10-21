using System;
using System.Linq;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Support.Interfaces;

namespace Fujitsu.AFC.Support.Tasks
{
    public class HistoryErrorLogMonitoring : ISupportTaskProcessor
    {
        private readonly ISupportService _supportService;
        private readonly IService<HistoryLog> _historyLogService;

        public HistoryErrorLogMonitoring(ISupportService supportService, IService<HistoryLog> historyLogService)
        {
            if (supportService == null)
            {
                throw new ArgumentNullException(nameof(supportService));
            }
            if (historyLogService == null)
            {
                throw new ArgumentNullException(nameof(historyLogService));
            }

            _supportService = supportService;
            _historyLogService = historyLogService;
        }

        public void Execute(Task task)
        {
            foreach (var errorEvent in _historyLogService.Query(x => x.Escalated == false).ToList())
            {
                _supportService.EscalateErrorEvent(errorEvent);
            }
        }
    }
}
