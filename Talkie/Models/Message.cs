using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Talkie.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public DateTime Modified { get; set; }
        public MessageType Type { get; set; }
        public MessageStatus Status { get; set; }

        [ForeignKey("User")]
        public string UserNumber { get; set; }

        [ForeignKey("Friend")]
        public string RecipientNumber { get; set; }

        public ICollection<Transaction>? Transactions
        { get; set; }

        public ICollection<Text>? Texts { get; set; }
        public ICollection<SharedFile>? SharedFiles { get; set; }

        public Account? User { get; set; }
    }
}