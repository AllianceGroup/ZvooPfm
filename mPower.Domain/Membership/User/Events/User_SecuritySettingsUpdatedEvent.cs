using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_SecuritySettingsUpdatedEvent : Event
    {
        public string UserId { get; set; }

        public bool EnableAdminAccess { get; set; }

        public bool EnableIntuitLogging { get; set; }
        public bool EnableAgentAccess { get; set; }
    }
}