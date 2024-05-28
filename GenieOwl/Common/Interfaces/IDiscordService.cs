namespace GenieOwl.Common.Interfaces
{
    using GenieOwl.Integrations.Entities;
    
    public interface IDiscordService
    {
        public Task<bool> GetAppsButtons(List<SteamApp> steamApps);

        public Task<bool> GetAppAchivementsButtons(SteamApp steamApp);
    }
}
