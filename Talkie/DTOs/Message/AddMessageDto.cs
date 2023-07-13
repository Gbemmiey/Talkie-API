using Talkie.Models;

namespace Talkie.DTOs.Message
{
    public class AddMessageDto
    {
        public MessageType Type { get; set; }
        public string RecipientNumber { get; set; } = string.Empty;

        public string Payload { get; set; } = string.Empty;

        public int AuthPin { get; set; } = 0;
    }
}