namespace GenieOwl.Integrations.Interfaces
{
    using GenieOwl.Utilities.Types;
    
    public interface IPerplexityIntegration
    {
        public Task<string> GetPerplexityChatResponse(string[] promptParameters, LenguageType lenguage);
    }
}
