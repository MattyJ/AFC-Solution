using System;

namespace Fujitsu.AFC.Model
{
    public class HistoryLog : TaskEntity
    {
        public int TaskId { get; set; }

        public string EventType { get; set; }

        public string EventDetail { get; set; }

        public DateTime CompletedDate { get; set; }

        public int? SiteId { get; set; }

        public bool? Escalated { get; set; }
    }
}
