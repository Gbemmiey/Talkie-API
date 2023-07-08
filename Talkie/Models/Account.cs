using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Talkie.Models
{
    public class Account
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public string Number { get; set; }

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public string EmailAddress { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Balance { get; set; }

        public byte[] AuthPin { get; set; }

        public List<Message>? Messages { get; set; }

        public List<Contact>? Contacts { get; set; }
    }
}