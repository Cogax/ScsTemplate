using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Generated
{
    [Index("Label", Name = "IX_TodoItems_Label", IsUnique = true)]
    [Index("Removed", Name = "IX_TodoItems_Removed", IsUnique = true)]
    public partial class TodoItems
    {
        [Key]
        public Guid Id { get; set; }
        public bool Completed { get; set; }
        [StringLength(250)]
        public string Label { get; set; } = null!;
        public int Version { get; set; }
        [Required]
        public bool? Removed { get; set; }
    }
}
