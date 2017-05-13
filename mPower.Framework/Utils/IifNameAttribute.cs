using System;
using System.ComponentModel;

namespace mPower.Framework.Utils
{
    public class IifNameAttribute : DescriptionAttribute
    {
        public IifNameAttribute()
        {
        }

        public IifNameAttribute(string description)
            : base(description)
        {
        }
    }
}
