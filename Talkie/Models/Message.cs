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
        public long Number { get; set; }

        [ForeignKey("Friend")]
        public long RecipientNumber { get; set; }

        public ICollection<Transaction>? Transactions { get; set; }
        public ICollection<Text>? Texts { get; set; }
        public ICollection<SharedFile>? SharedFiles { get; set; }

        public Account? User { get; set; }
        //        public Account? Friend { get; set; }
    }
}