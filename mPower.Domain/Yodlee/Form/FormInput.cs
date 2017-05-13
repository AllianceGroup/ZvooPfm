using System.Collections.Generic;

namespace mPower.Domain.Yodlee.Form
{
    public class FormInput
    {
        public string Label { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public List<SelectOption> SelectOptions { get; set; }
    }
}