using mPower.Aggregation.Contract.Documents;
using mPower.Domain.Yodlee.Form;
using System;
using System.Collections.Generic;

namespace Default.Models
{
    public class AuthenticateToInstitutionViewModel
    {
        private string _institutionName;

        public long InstitutionId { get; set; }
        public IList<KeyDocument> Keys { get; set; }
        public string InstitutionName
        {
            get { return _institutionName; }
            set
            {
                _institutionName = value;
                var arr =
                    value
                    .Replace("Union", "")
                    .Replace("Investments", "")
                    .Replace("Credit", "")
                    .SplitByLength(13, ' ','-');
                Line1 = arr[0];
                if (Line1.Length > 13)
                {
                    InstitutionNameFontSize = 25;
                }
                if (arr.Count> 1)
                {
                    Line2 = arr[1];
                    if (Line2.Length > 13)
                    {
                        InstitutionNameFontSize = 25;
                    }
                }
            }
        }

        public int InstitutionNameFontSize = 30;
        public string Line1 { get; set; }
        public string Line2 { get; set; }
    }
}