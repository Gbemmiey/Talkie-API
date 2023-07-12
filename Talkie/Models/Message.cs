using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Talkie.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public DateTime Modified { get; set; }
        public MessageType Type { get; set; }

        [ForeignKey("User")]
        public string Number { get; set; } = string.Empty;

        [ForeignKey("Friend")]
        public string RecipientNumber { get; set; } = string.Empty;

        public ICollection<Transaction>? Transactions { get; set; }
        public ICollection<Text>? Texts { get; set; }
        public ICollection<SharedFile>? SharedFiles { get; set; }

        public Account? User { get; set; }
    }
}