using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Poc.Nsb.Outbox.Infrastructure.Adapters.Persistence.Generated
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
