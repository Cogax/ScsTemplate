using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Generated
{
    [Table("Schema", Schema = "HangFire")]
    public partial class Schema
    {
        [Key]
        public int Version { get; set; }
    }
}
