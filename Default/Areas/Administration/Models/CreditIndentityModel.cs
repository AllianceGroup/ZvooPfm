using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default.Areas.Administration.Models
{
    public class CreditIndentityModel
    {
        public string Ssn { get; set; }

        public string ClientKeyEncrypted { get; set; }

        public bool IsEnrolled { get; set; }
    }
}