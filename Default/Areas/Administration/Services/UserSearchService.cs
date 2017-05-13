using System;
using mPower.Documents.Documents.Membership;

namespace Default.Areas.Administration.Services
{
    public static class UserSearchService
    {
        public static bool UserMatches(UserDocument user, string searchKey)
        {
            bool firstNameMatch = false;
            bool lastNameMatch = false;
            bool userNameMatch = false;
            bool refCodeMatch = false;

            if (user.FirstName != null)
            {
                firstNameMatch = user.FirstName.Contains(searchKey);
            }

            if (user.LastName != null)
            {
                lastNameMatch = user.LastName.ToLower().Contains(searchKey);
            }

            if (user.UserName != null)
            {
                userNameMatch = user.UserName.ToLower().Contains(searchKey);
            }

            if (user.ReferralCode != null)
            {
                refCodeMatch = user.ReferralCode.Equals(searchKey, StringComparison.InvariantCultureIgnoreCase);
            }

            return firstNameMatch || lastNameMatch || userNameMatch || refCodeMatch;
        }

        public static bool UserMatchesApplication(UserDocument user, string applicationId)
        {
            bool applicationIdMatch = false;

            if (user.ApplicationId != null)
            {
                applicationIdMatch = user.ApplicationId == applicationId;
            }

            return applicationIdMatch;
        }
    }
}