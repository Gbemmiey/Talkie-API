using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Talkie.Models
{
    public class SharedFile
    {
        public int Id { get; set; }

        public int MessageId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public Message? Message { get; set; }
    }
}