using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Default.Factories.ViewModels.Aggregation;

namespace mPower.WebApi.Tenants.ViewModels.Aggregation
{
    public class ReauthenticateToInstitutVewModel
    {        
        public long IntuitAccountId { get; set; }
        public AuthentificateToInstitutionDto Dto { get; set; }
    }
}
