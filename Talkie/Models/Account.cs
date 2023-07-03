using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Talkie.Models
{
    public class Account
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public string Number { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Balance { get; set; }

        public byte[] TransactPin { get; set; }

        public List<Message>? Messages { get; set; }
        public List<Contact>? Contacts { get; set; }
    }
}