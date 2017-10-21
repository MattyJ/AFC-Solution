using System;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.AFC.Model
{
    public abstract class TaskEntity : BaseEntity, IBaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Handler { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string Frequency { get; set; }

        public int? Pin { get; set; }

        public int? ProjectId { get; set; }

        [StringLength(100)]
        public string ProjectName { get; set; }

        public int? CaseId { get; set; }

        [StringLength(100)]
        public string SiteTitle { get; set; }

        [StringLength(100)]
        public string CaseTitle { get; set; }

        public string Dictionary { get; set; }

        public bool? IsPrimary { get; set; }

        public int? CurrentProjectId { get; set; }

        public int? NewProjectId { get; set; }

        public int? FromPin { get; set; }

        public int? ToPin { get; set; }

        public int? CurrentCaseId { get; set; }

        public int? NewCaseId { get; set; }
    }
}
