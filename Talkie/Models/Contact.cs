namespace Talkie.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string UserNumber { get; set; } = string.Empty;
        public string BeneficiaryNumber { get; set; } = string.Empty;

        public Account? User { get; set; }
    }
}