using System.Collections.Generic;
using mPower.Aggregation.Contract.Documents;

namespace Default.Factories.ViewModels.Aggregation
{
    public class AuthentificateToInstitutionDto
    {
        public long InstitutionId { get; set; }

        public List<KeyDocument> Keys { get; set; }
    }
}
