using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Generated
{
    [Table("Hash", Schema = "HangFire")]
    public partial class Hash
    {
        [Key]
        [StringLength(100)]
        public string Key { get; set; } = null!;
        [Key]
        [StringLength(100)]
        public string Field { get; set; } = null!;
        public string? Value { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
