namespace GenieOwl.Common.Services
{
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Integrations.Entities;
    using GenieOwl.Integrations;
    using Microsoft.Extensions.Configuration;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;

    public class SteamService : ISteamService
    {
        private static IConfiguration _Configuration;

        private readonly SteamIntegration _SteamIntegration;

        private readonly List<SteamApp> _SteamApps;

        private readonly List<SteamApp> _SteamAppsWithDlcs;

        public SteamService(IConfiguration configuration)
        {
            _Configuration = configuration;
            _SteamIntegration = new SteamIntegration(_Configuration);

            _SteamApps = _SteamIntegration.GetApps();
            _SteamAppsWithDlcs = _SteamIntegration.GetAppsWithDlcs();
        }

        public SteamApp? GetSteamAppByName(string appName)
        {
            List<SteamApp> appsMatchesByName;

            if (_SteamApps.Count > 0)
            {
                appsMatchesByName = GetAppsMatchesByAppName(appName, _SteamApps);

                if (appsMatchesByName.Count > 0)
                {
                    return _SteamIntegration.GetAppAchievements(appsMatchesByName.FirstOrDefault());
                }
            }

            if (_SteamAppsWithDlcs.Count > 0)
            {
                appsMatchesByName = GetAppsMatchesByAppName(appName, _SteamAppsWithDlcs);
                
                if (appsMatchesByName.Count > 0)
                {
                    return _SteamIntegration.GetAppAchievements(appsMatchesByName.FirstOrDefault());
                }
            }

            return null;
        }

        public List<SteamApp> GetSteamAppsByMatches(string appName)
        {
            List<SteamApp> steamAppsDlcsMatches = GetAppsMatchesByAppName(appName, _SteamAppsWithDlcs);

            var asd = _SteamIntegration.GetAppAchievements(steamAppsDlcsMatches.FirstOrDefault());


            if (steamAppsDlcsMatches.Count > 0)
                return steamAppsDlcsMatches;
            
            List<SteamApp> steamAppsMatches = GetAppsMatchesByAppName(appName, _SteamApps);
            if (steamAppsMatches.Count > 0)
                return steamAppsMatches;

            throw new Exception("Game couldn't not be found");
        }

        private static List<SteamApp> GetAppsMatchesByAppName(string appName, List<SteamApp> steamApps)
        {
            return steamApps.Select(app => new
            {
                SteamApp = app,
                Matches = Regex.Matches(app.Name?.ToLower(), $@"\b{Regex.Escape(appName.ToLower())}\b").Count
            })
            .Where(x => x.Matches > 0)
            .OrderByDescending(x => x.Matches)
            .Select(x => x.SteamApp)
            .ToList();
        }

        private static string ReplaceSpecialCharacters(string input)
        {
            return Regex.Replace(input, @"[^\w\s]", "");
        }
    }
}
