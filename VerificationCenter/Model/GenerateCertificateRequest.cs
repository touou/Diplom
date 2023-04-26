using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificationCenter.Model
{
    public class GenerateCertificateRequest
    {
        public string Country { get; set; }
        public string Inn { get; set; }
        public string locality { get; set; }
        public string Organization { get; set; }
        public string OrganizationalUnit { get; set; }
        public string CommonName { get; set; }
        public string Password { get; set; }
    }
}
