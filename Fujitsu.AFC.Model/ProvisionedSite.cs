using System.ComponentModel.DataAnnotations;

namespace Fujitsu.AFC.Model
{
    public class ProvisionedSite : BaseEntity, IBaseEntity
    {
        public int Id { get; set; }

        public bool IsAllocated { get; set; } = false;

        [Required]
        [StringLength(36)]
        public string Name { get; set; }
        public string Url { get; set; }

        public int ProvisionedSiteCollectionId { get; set; }
        public virtual ProvisionedSiteCollection ProvisionedSiteCollection { get; set; }
    }
}