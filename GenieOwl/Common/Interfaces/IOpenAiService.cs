namespace GenieOwl.Common.Interfaces
{
    using GenieOwl.Integrations.Entities;
    
    public interface IOpenAiService
    {
        public Task<string> GetChatResponse(SteamApp steamApp);
        public Task<string> GetChatResponse(SteamAppAchievement appAchievements);
    }
}
