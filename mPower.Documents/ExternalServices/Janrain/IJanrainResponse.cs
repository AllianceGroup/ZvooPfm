namespace mPower.Documents.ExternalServices.Janrain
{
    public interface IJanrainResponse
    {
        JanrainProviderType Provider { get; set; }

        string Identifier { get; set; }
    }
}
