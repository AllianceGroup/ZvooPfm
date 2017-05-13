using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Default.Factories.Commands.Aggregation
{
   public class ReauthenticateDto
    {
        public long InstitutionId { get; set; }

        public long IntuitAccountId { get; set; }

        public string UserId { get; set; }

        public bool AggregationLoggingEnabled { get; set; }
    }
}
