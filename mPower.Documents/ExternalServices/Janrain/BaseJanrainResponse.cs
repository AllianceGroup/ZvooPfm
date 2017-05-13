namespace mPower.Documents.ExternalServices.Janrain
{
    public class BaseJanrainResponseProfile
    {
        public string providerName { get; set; }
    }

    public class BaseJanrainResponse
    {
        public string stat { get; set; }

        public BaseJanrainResponseProfile profile { get; set; }
    }
}
