using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fujitsu.AFC.Model
{
    public class ProvisionedSiteCollection : BaseEntity, IBaseEntity
    {
        public int Id { get; set; }

        [Index(IsUnique = true)]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
