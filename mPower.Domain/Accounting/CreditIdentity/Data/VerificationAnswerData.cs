namespace mPower.Domain.Accounting.CreditIdentity.Data
{
    public class VerificationAnswerData
    {
        public bool IsCorrect { get; set; }
        public string Answer { get; set; }
        public int SequenceNumber { get; set; }
    }
}