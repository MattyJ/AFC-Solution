using System;

namespace Fujitsu.AFC.Model
{
    public class Task : TaskEntity
    {
        public DateTime? CompletedDate { get; set; }

        public DateTime? NextScheduledDate { get; set; }

        public int? SiteId { get; set; }
    }
}
