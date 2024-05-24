namespace GenieOwl.Integrations.Interfaces
{
    using GenieOwl.Integrations.Entities;
    using System.Collections.Generic;

    public interface ISteamIntegration
    {
        public List<SteamApp> GetApps();

        public List<SteamApp> GetAppsWithDlcs();

        public List<SteamAppNews> GetNewsByApp(string appId);

        public SteamApp GetAppAchievements(SteamApp steamApp);
    }
}
