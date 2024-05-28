namespace GenieOwl.Common.Interfaces
{
    using Discord.Commands;

    public interface ISteamService
    {
        public Task<bool> GetSteamAppsByMatches(string appName, SocketCommandContext context);
    }
}
