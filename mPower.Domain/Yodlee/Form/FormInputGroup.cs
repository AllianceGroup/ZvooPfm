using System.Collections.Generic;

namespace mPower.Domain.Yodlee.Form
{
    public class FormInputGroup
    {
        public List<FormInputGroup> FormInputGroups { get; set; }
        public List<FormInput> FormInputs { get; set; }
        public LayoutType Layout { get; set; }
    }

    public enum LayoutType
    {
        Standard,
        LeftToRight,
        OptionalGroupings
    }
}