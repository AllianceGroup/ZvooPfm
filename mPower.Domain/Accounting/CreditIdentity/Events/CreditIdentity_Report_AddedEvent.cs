using Paralect.Domain;
using mPower.Domain.Accounting.CreditIdentity.Data;

namespace mPower.Domain.Accounting.CreditIdentity.Events
{
    public class CreditIdentity_Report_AddedEvent : Event
    {
        public string UserId { get; set; }

        public string UserFullName { get; set; }

        public string ClientKey { get; set; }

        public string CreditReportId { get; set; }

        public string CreditScoreId { get; set; }

        public string SocialSecurityNumber { get; set; }

        public CreditReportData Data { get; set; }
    }
}