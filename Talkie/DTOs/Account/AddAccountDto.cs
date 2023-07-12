namespace Talkie.DTOs.Account
{
    public class AddAccountDto
    {
        public string Number { get; set; } = string.Empty;

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public decimal Balance { get; set; }
        public string EmailAddress { get; set; } = string.Empty;

        public string ProfilePictureUrl { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public int AuthPin { get; set; }
    }
}