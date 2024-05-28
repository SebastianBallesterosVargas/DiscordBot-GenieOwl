namespace GenieOwl.Integrations.Interfaces
{
    public interface IOpenAiIntegration
    {
        public Task<string> GetChatResponseByOpenAi(string prompt);
    }
}
