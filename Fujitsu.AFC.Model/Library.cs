using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fujitsu.AFC.Model
{
    public class Library : BaseEntity, IBaseEntity
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }
        [Index(IsUnique = true)]
        public int CaseId { get; set; }
        public string Title { get; set; }
        public int ProjectId { get; set; }
        public Guid ListId { get; set; }
        public bool IsClosed { get; set; }
        public string Url { get; set; }
        public string Dictionary { get; set; }
    }
}
