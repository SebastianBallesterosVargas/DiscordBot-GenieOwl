namespace GenieOwl.Common.Interfaces
{
    using GenieOwl.Integrations.Entities;
    
    public interface ISteamService
    {
        public SteamApp? GetSteamAppByName(string appName);
    }
}
