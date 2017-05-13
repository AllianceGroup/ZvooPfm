using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.CreditIdentity.Data;

namespace mPower.Domain.Accounting.CreditIdentity.Events
{
    public class CreditIdentity_Questions_SetEvent : Event
    {
        public string ClientKey { get; set; }

        public List<VerificationQuestionData> Questions { get; set; }
    }
}