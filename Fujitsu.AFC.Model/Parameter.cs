using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fujitsu.AFC.Model
{
    public class Parameter : BaseEntity, IBaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Index(IsUnique = true)]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        [Required]
        [StringLength(250)]
        public string Description { get; set; }
    }
}
