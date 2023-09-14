using Talkie.Models;

namespace Talkie.DTOs.Message
{
    public class GetSentMessageDto
    {
        public DateTime Modified { get; set; }
        public MessageType Type { get; set; }
        public long RecipientNumber { get; set; }

        public string? Payload { get; set; }
    }
}