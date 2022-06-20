using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Poc.Nsb.Outbox.Infrastructure.Adapters.Persistence.Generated
{
    [Keyless]
    [Table("Poc.Nsb.Outbox.Web.WebOutbox.Delayed")]
    [Index("Due", Name = "Index_Due")]
    public partial class PocNsbOutboxWebWebOutboxDelayed
    {
        public string Headers { get; set; } = null!;
        public byte[]? Body { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Due { get; set; }
        public long RowVersion { get; set; }
    }
}
