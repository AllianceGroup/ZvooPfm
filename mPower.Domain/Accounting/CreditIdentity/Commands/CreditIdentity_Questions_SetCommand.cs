using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.CreditIdentity.Data;

namespace mPower.Domain.Accounting.CreditIdentity.Commands
{
    public class CreditIdentity_Questions_SetCommand : Command
    {
        public string ClientKey { get; set; }

        public List<VerificationQuestionData> Questions { get; set; }
    }
}