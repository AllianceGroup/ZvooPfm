using System;
using System.Collections.Generic;

namespace Default.ViewModel.Areas.Credit.Report
{
    public class PersonalInfo
    {
        public DateTime DateOfBirth { get; set; }
        public IEnumerable<String> PhoneNumbers { get; set; }
        public List<String> Employers { get; set; }

        public IEnumerable<String> Inquiries { get; set; }
        public String BillingAddress { get; set; }
        public IEnumerable<String> PreviousAddresses { get; set; }

    }
}
