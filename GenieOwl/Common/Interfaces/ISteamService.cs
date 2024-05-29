namespace GenieOwl.Common.Interfaces
{
    using GenieOwl.Integrations.Entities;

    public interface ISteamService
    {
        public List<SteamApp> GetSteamAppsByMatches(string appName);
    }
}
