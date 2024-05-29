namespace GenieOwl.Common.Interfaces
{
    using Discord.Commands;
    using GenieOwl.Integrations.Entities;
    
    public interface IDiscordService
    {
        public Task<bool> GetAppsButtons(List<SteamApp> steamApps, SocketCommandContext context);

        public Task<bool> GetAppAchivementsButtons(SteamApp steamApp, SocketCommandContext context);
    }
}
