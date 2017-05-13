using Default.Factories.ViewModels.Aggregation;
using mPower.Aggregation.Contract.Data;
using mPower.WebApi.Utilities;
using Newtonsoft.Json;

namespace mPower.WebApi.Tenants.ViewModels.Aggregation
{
    public class AuthenticateToInstitutVewModel
    {
        public AuthentificateToInstitutionDto Dto { get; set; }

        [JsonConverter(typeof(MfaSessionConverter))]
        public MfaSession MfaSession { get; set; }
    }
}