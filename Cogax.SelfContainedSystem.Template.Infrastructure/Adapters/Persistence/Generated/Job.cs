using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Generated
{
    [Table("Job", Schema = "HangFire")]
    public partial class Job
    {
        public Job()
        {
            JobParameter = new HashSet<JobParameter>();
            State = new HashSet<State>();
        }

        [Key]
        public long Id { get; set; }
        public long? StateId { get; set; }
        [StringLength(20)]
        public string? StateName { get; set; }
        public string InvocationData { get; set; } = null!;
        public string Arguments { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ExpireAt { get; set; }

        [InverseProperty("Job")]
        public virtual ICollection<JobParameter> JobParameter { get; set; }
        [InverseProperty("Job")]
        public virtual ICollection<State> State { get; set; }
    }
}
