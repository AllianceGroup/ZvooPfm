using System;
using System.ComponentModel;
using System.Linq;

namespace mPower.Framework.Utils.Extensions
{
    public static class AttributeExtension
    {
        public static T GetAttribute<T>(this object value) where T : Attribute
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi == null) return null;
            var attributes = (T[])fi.GetCustomAttributes(typeof(T), false);

            return attributes.SingleOrDefault(x => x.GetType() == typeof(T));
        }

        public static string GetDescription(this object value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string GetNullableDescription(this object value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? null : attribute.Description;
        }
    }
}