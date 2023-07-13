namespace Talkie.DTOs.Contact
{
    public class GetContactDto
    {
        public string BeneficiaryNumber { get; set; }
        public string AccountName { get; set; } = string.Empty;

        public string ProfilePicture { get; set; } = string.Empty;
    }
}