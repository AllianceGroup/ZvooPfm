using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mPower.Aggregation.Contract.Data;

namespace Default.Models
{
    public class MfaChallengeViewModel
    {
        public long InstitutionId { get; set; }
        public string SessionToken { get; set; }
        public IEnumerable<AggregationQuestion> Questions { get; set; }
    }
}
