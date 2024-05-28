namespace GenieOwl.Common.Interfaces
{
    public interface IOpenAiService
    {
        public Task GetChatResponse(string prompt);
    }
}
