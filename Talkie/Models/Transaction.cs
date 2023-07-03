using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Talkie.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int MessageId { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Amount { get; set; }

        public Message? Message { get; set; }
    }
}