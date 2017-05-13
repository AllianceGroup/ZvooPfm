using System.Collections.Generic;

namespace Default
{
    public class Constants
    {
        public static readonly string ReadModeKey = "IsReadOnlyMode";

        public class Validation
        {
            public class Regexps
            {
                public const string Email = @"^[-a-z0-9!#$%&'*+/=?^_`{|}~]+(\.[-a-z0-9!#$%&'*+/=?^_`{|}~]+)*@([a-z0-9]([-a-z0-9]{0,61}[a-z0-9])?\.)*(aero|arpa|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|mobi|museum|name|net|org|pro|tel|travel|[a-z][a-z])$";

                public const string Password = @"^[\S]{6,20}$";
            }

            public class Messages
            {
                public const string InvalidEmail = "Please input valid email address.";

                public const string InvalidPassword = "Invalid new password.";

                public const string InvalidPasswordConfirmation = "New password and it's confirmation should be the same.";
            }
        }

        public class CommonData
        {
            public static readonly Dictionary<int, string> SequrityLevels = new Dictionary<int, string>
            {
                {0, ""}
            };

            public static readonly List<string> SequrityQuestions = new List<string>
            {
                "What is your mother's maiden name?",
                "What was your childhood nickname?",
                "What street did you live on in third grade?",
                "In what city or town was your first job?", 
                "What was the name of your first stuffed animal?"
            };

            public const string SessionParamName = "ASPSESSID";
            public const string SessionCookieName = "ASP.NET_SESSIONID";
            public const string AuthParamName = "AUTHID";
        }

        public class Upload
        {
            public const string FileName = "Filedata";
        }

        public class Images
        {
            public const int MaxImageWidth = 166;
            public const int MaxImageHeight = 166;

            public const int GoalCreateAvatarWidth = 63;
            public const int GoalCreateAvatarHeight = 63;

            public const int GoalListingThumbnailWidth = 78;
            public const int GoalListingThumbnailHeight = 78;

            public const int GoalCustomAvatarWidth = 166;
            public const int GoalCustomAvatarHeight = 166;
        }
    }
}