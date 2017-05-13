using System.Collections.Generic;

namespace Default.Models
{
    public class AlertsListModel
    {
        public List<UserEmail> Emails { get; set; }

        public List<string> Phones { get; set; }

        public List<AlertModel> Alerts { get; set; }
    }

    public class UserEmail
    {
        public string Value { get; set; }

        public bool IsMain { get; set; }

        public UserEmail(string email)
        {
            Value = email;
        }
    }
}