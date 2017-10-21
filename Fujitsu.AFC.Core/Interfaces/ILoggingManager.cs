using Fujitsu.AFC.Enumerations;

namespace Fujitsu.AFC.Core.Interfaces
{
    public interface ILoggingManager
    {
        void Write(LoggingEventSource loggingEventSource,
            LoggingEventType loggingEventType,
            string message);
    }
}
