using System;
using mPower.Framework.Utils.Notification;

namespace mPower.Framework.Utils.Extensions
{
    public static class EnumExtensions
    {
        public static string GetIifName(this Enum value)
        {
            var attribute = value.GetAttribute<IifNameAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static NotificationGroupEnum GetNotificationGroup(this Enum value)
        {
            var attribute = value.GetAttribute<NotificationGroupAttribute>();
            return attribute == null ? NotificationGroupEnum.System : attribute.Group;
        }
    }
}
