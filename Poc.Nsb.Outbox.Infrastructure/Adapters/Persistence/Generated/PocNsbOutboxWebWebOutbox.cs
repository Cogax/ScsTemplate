using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Poc.Nsb.Outbox.Infrastructure.Adapters.Persistence.Generated
{
    [Keyless]
    [Table("Poc.Nsb.Outbox.Web.WebOutbox")]
    [Index("RowVersion", Name = "Index_RowVersion")]
    public partial class PocNsbOutboxWebWebOutbox
    {
        public Guid Id { get; set; }
        [StringLength(255)]
        [Unicode(false)]
        public string? CorrelationId { get; set; }
        [StringLength(255)]
        [Unicode(false)]
        public string? ReplyToAddress { get; set; }
        public bool Recoverable { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Expires { get; set; }
        public string Headers { get; set; } = null!;
        public byte[]? Body { get; set; }
        public long RowVersion { get; set; }
    }
}
