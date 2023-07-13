using Talkie.Models;

namespace Talkie.DTOs.Message
{
    public class GetMessageDto
    {
        public int Id { get; set; }
        public DateTime Modified { get; set; }
        public MessageType Type { get; set; }
        public string RecipientNumber { get; set; }

        public string? Payload { get; set; }

        public string? Interaction { get; set; }
    }
}