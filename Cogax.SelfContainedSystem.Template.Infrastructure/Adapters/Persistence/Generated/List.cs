using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Generated
{
    [Table("List", Schema = "HangFire")]
    public partial class List
    {
        [Key]
        public long Id { get; set; }
        [Key]
        [StringLength(100)]
        public string Key { get; set; } = null!;
        public string? Value { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ExpireAt { get; set; }
    }
}
