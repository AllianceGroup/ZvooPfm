using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mPower.Aggregation.Contract.Data;
using mPower.WebApi.Utilities;
using Newtonsoft.Json;

namespace mPower.WebApi.Tenants.ViewModels.Aggregation
{
    public class RefreshAccountViewModel
    {
        public long FinicityAccountId { get; set; }

        [JsonConverter(typeof(MfaSessionConverter))]
        public MfaSession MfaSession { get; set; }
    }
}
