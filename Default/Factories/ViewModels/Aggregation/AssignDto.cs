using System.Collections.Generic;
using mPower.Aggregation.Contract.Documents;

namespace Default.Factories.ViewModels.Aggregation
{
    public class AssignDto
    {
        public string LedgerId { get; set; }
        public string UserId { get; set; }
        public IList<AccountDocument> Accounts { get; set; }
        public long InstitutionId { get; set; }
    }
}