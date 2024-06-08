namespace GenieOwl.Common.Interfaces
{
    using Discord.Commands;
    using GenieOwl.Integrations.Entities;
    using GenieOwl.Utilities.Types;

    public interface IDiscordService
    {
        public Task GetAppsButtons(List<SteamApp> steamApps, SocketCommandContext context, LenguageType lenguage);

        public Task GetAppAchivementsButtons(SteamApp steamApp, SocketCommandContext context, LenguageType lenguage);
    }
}
