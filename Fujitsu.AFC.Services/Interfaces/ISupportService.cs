using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Services.Interfaces
{
    public interface ISupportService
    {
        void EscalateErrorEvent(HistoryLog historyLog);
    }
}
