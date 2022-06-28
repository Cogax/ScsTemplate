using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Generated
{
    [Table("JobParameter", Schema = "HangFire")]
    public partial class JobParameter
    {
        [Key]
        public long JobId { get; set; }
        [Key]
        [StringLength(40)]
        public string Name { get; set; } = null!;
        public string? Value { get; set; }

        [ForeignKey("JobId")]
        [InverseProperty("JobParameter")]
        public virtual Job Job { get; set; } = null!;
    }
}
