using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Generated
{
    [Table("AggregatedCounter", Schema = "HangFire")]
    public partial class AggregatedCounter
    {
        [Key]
        [StringLength(100)]
        public string Key { get; set; } = null!;
        public long Value { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ExpireAt { get; set; }
    }
}
