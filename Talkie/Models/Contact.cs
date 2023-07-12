using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Talkie.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string UserNumber { get; set; }
        public string BeneficiaryNumber { get; set; }

        public Account? User { get; set; }
    }
}