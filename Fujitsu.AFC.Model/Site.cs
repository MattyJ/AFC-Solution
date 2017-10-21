using System.ComponentModel.DataAnnotations.Schema;

namespace Fujitsu.AFC.Model
{
    public class Site : BaseEntity, IBaseEntity
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        [Index(IsUnique = true)]
        public int Pin { get; set; }
        public int ProvisionedSiteId { get; set; }
        public virtual ProvisionedSite ProvisionedSite { get; set; }
        public bool RestrictedUser { get; set; }
        public int? PrimaryProjectId { get; set; }
        public string Dictionary { get; set; }
    }
}
