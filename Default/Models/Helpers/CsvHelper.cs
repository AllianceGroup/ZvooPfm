using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mPower.Documents.Documents.Membership;

namespace Default.Helpers
{
    public static class CsvHelper
    {
        public static string CreateUsersCsv(IEnumerable<UserDocument> users)
        {

            var sb = new StringBuilder();
            sb.AppendFormat("Report Created On: {0} \n", DateTime.Now.ToShortDateString());
            sb.AppendLine("Created On, First Name, Last Name, UserName, Email, Phone, Is Active, Zip Code, Date of Birth, Gender, Last Login, Credit Identities, Aggregated Accounts\n");

            foreach (var user in users)
            {
                sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}\n",
                    user.CreateDate,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.UserName,
                    user.Phones.FirstOrDefault(),
                    user.IsActive,
                    user.ZipCode,
                    FormatDate(user.BirthDate),
                    user.Gender,
                    FormatDateTime(user.LastLoginDate),
                    user.Identities.Count(),
                    user.AccountsAggregated);
            }
            return sb.ToString();
        }

        private static string FormatDate(DateTime? date)
        {
            return (date == null) ? "-" : date.Value.Date.ToShortDateString();
        }

        private static string FormatDateTime(DateTime date)
        {
            return date == DateTime.MinValue ? "-" : date.ToString();
        }
    }
}