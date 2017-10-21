using System;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.AFC.Model
{
    public class TimerLock : BaseEntity, IBaseEntity
    {
        public int Id { get; set; }

        [Required]
        public Guid LockedInstance { get; set; }

        [Required]
        public int LockedPin { get; set; }

        public int TaskId { get; set; }
        public virtual Task Task { get; set; }
    }
}
