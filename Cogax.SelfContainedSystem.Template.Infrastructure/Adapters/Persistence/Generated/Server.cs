using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Generated
{
    [Table("Server", Schema = "HangFire")]
    [Index("LastHeartbeat", Name = "IX_HangFire_Server_LastHeartbeat")]
    public partial class Server
    {
        [Key]
        [StringLength(200)]
        public string Id { get; set; } = null!;
        public string? Data { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastHeartbeat { get; set; }
    }
}
