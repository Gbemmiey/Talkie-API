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