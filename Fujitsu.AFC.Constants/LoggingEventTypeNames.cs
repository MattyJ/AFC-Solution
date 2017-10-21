using Fujitsu.AFC.Enumerations;

namespace Fujitsu.AFC.Constants
{
    public struct LoggingEventTypeNames
    {
        public static readonly string Information;
        public static readonly string Warning;
        public static readonly string Error;

        static LoggingEventTypeNames()
        {
            Information = LoggingEventType.Information.ToString();
            Warning = LoggingEventType.Warning.ToString();
            Error = LoggingEventType.Error.ToString();
        }
    }
}
