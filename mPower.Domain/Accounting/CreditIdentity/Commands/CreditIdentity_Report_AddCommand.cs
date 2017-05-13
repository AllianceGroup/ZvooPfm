using Paralect.Domain;
using mPower.Domain.Accounting.CreditIdentity.Data;

namespace mPower.Domain.Accounting.CreditIdentity.Commands
{
    public class CreditIdentity_Report_AddCommand : Command
    {
        public string ClientKey { get; set; }

        public string CreditReportId { get; set; }

        public string CreditScoreId { get; set; }

        public CreditReportData Data { get; set; }
    }
}