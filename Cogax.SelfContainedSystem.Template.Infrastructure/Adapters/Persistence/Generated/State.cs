using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Generated
{
    [Table("State", Schema = "HangFire")]
    public partial class State
    {
        [Key]
        public long Id { get; set; }
        [Key]
        public long JobId { get; set; }
        [StringLength(20)]
        public string Name { get; set; } = null!;
        [StringLength(100)]
        public string? Reason { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        public string? Data { get; set; }

        [ForeignKey("JobId")]
        [InverseProperty("State")]
        public virtual Job Job { get; set; } = null!;
    }
}
