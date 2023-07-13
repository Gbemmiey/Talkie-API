namespace Talkie.DTOs.Message
{
    public class GetStatementDto
    {
        public int Id { get; set; }

        public DateTime Modified { get; set; }

        public string? ContactNumber { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public string? Payload { get; set; }

        public string? Interaction { get; set; }

        public string? Number { get; set; }

        public string? RecipientNumber { get; set; }
    }
}