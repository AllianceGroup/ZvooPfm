namespace mPower.Domain.Membership.User.Data
{
    public class RealestateData
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public long AmountInCents { get; set; }

        public RealestateRawData RawData { get; set; }
    }
}