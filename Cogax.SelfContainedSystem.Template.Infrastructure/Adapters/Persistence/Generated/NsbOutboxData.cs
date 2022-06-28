using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Generated
{
    [Table("NSB_OutboxData")]
    public partial class NsbOutboxData
    {
        [Key]
        [StringLength(200)]
        public string MessageId { get; set; } = null!;
        public bool Dispatched { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DispatchedAt { get; set; }
        [StringLength(23)]
        [Unicode(false)]
        public string PersistenceVersion { get; set; } = null!;
        public string Operations { get; set; } = null!;
    }
}
