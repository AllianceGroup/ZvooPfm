using mPower.Documents.Documents.Credit.CreditIdentity;
using mPower.Documents.Documents.Membership;

namespace Default.Factories.Commands.Credit
{
    public class EnrollmentDto
    {
        public CreditIdentityDocument CreditIdentity { get; set; }

        public UserDocument User { get; set; }
    }
}