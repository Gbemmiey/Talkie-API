using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Talkie.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public long UserNumber { get; set; }
        public long BeneficiaryNumber { get; set; }

        public Account? User { get; set; }
    }
}