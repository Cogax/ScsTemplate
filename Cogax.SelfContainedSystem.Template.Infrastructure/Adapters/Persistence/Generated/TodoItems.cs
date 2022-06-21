using System.ComponentModel.DataAnnotations;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Generated
{
    public partial class TodoItems
    {
        [Key]
        public Guid Id { get; set; }
        public bool Completed { get; set; }
        [StringLength(250)]
        public string Label { get; set; } = null!;
    }
}
