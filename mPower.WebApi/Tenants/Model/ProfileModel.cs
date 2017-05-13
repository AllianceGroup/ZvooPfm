using Default.Models;

namespace mPower.WebApi.Tenants.Model
{
    public class ProfileModel
    {
        public UserDetailsModel UserDetails { get; set; }

        public SecurityQuestionModel SecurityQuestion { get; set; }

        public SecurityLevelModel SecurityLevel { get; set; }

        public SecuritySettingsModel SecuritySettings { get; set; }
    }
}