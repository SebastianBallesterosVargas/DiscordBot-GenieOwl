namespace GenieOwl.Common.Services
{
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Integrations.Entities;
    using GenieOwl.Integrations.Interfaces;
    using GenieOwl.Utilities.Messages;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;

    public class SteamService : ISteamService
    {
        /// <summary>
        /// Instancia de la integracion de Steam con acceso a SteamAPI
        /// </summary>
        private static ISteamIntegration _SteamIntegration;

        /// <summary>
        /// Lista de aplicaciones de Steam sin Dlcs
        /// </summary>
        private static List<SteamApp> _SteamApps;

        /// <summary>
        /// Lista de aplicaciones de Steam con Dlcs
        /// </summary>
        private static List<SteamApp> _SteamAppsWithDlcs;

        public SteamService(ISteamIntegration steamIntegration)
        {
            _SteamIntegration = steamIntegration;

            //_SteamApps = _SteamIntegration.GetApps();
            //this._SteamAppsWithDlcs = this._SteamIntegration.GetAppsWithDlcs();
        }

        /// <summary>
        /// Obtiene las aplicaciones de Steam que coincidan con la búsqueda dentro de SteamAPI
        /// </summary>
        /// <param name="appName">Nombre de la búsqueda</param>
        /// <returns>Aplicaciones de Steam</returns>
        public List<SteamApp> GetSteamAppsByMatches(string appName)
        {
            List<SteamApp> appsMatches = GetAppsMatchesByAppName(appName, _SteamApps);
            if (appsMatches.Count == 1)
                return GetAchievementsForSteamApp(appsMatches.FirstOrDefault());

            if (appsMatches.Count > 1)
                return appsMatches;

            List<SteamApp> appsDlcsMatches = GetAppsMatchesByAppName(appName, _SteamAppsWithDlcs);
            if (appsDlcsMatches.Count == 1)
                return GetAchievementsForSteamApp(appsDlcsMatches.FirstOrDefault());

            if (appsDlcsMatches.Count > 1)
                return appsDlcsMatches;

            throw CustomMessages.ResponseMessageEx(MessagesType.AppNotFound);
        }

        /// <summary>
        /// Obtiene los logros para una aplicación de Steam
        /// </summary>
        /// <param name="steamApp">Aplicación de Steam a obtener logros</param>
        /// <returns>Aplicación de Steam con logros</returns>
        private static List<SteamApp> GetAchievementsForSteamApp(SteamApp steamApp)
        {
            SteamApp appAchievements = _SteamIntegration.GetAppAchievements(steamApp);

            if (appAchievements.Achievements == null || appAchievements.Achievements.Count == 0)
            {
                throw CustomMessages.ResponseMessageEx(MessagesType.AppNotAchivement, steamApp.Name);
            }

            return new List<SteamApp>() { appAchievements };
        }


        /// <summary>
        /// Busca en las aplicaciones de Steam que coincidan con el appName
        /// </summary>
        /// <param name="appName">Nombre de la aplicación buscada</param>
        /// <param name="steamApps">Aplicaciones de Steam a buscar</param>
        /// <returns>Coincidencias por nombre de aplicación</returns>
        private static List<SteamApp> GetAppsMatchesByAppName(string appName, List<SteamApp> steamApps)
        {
            string cleanedAppName = Regex.Replace(appName.ToLower(), @"[:;.,_®@-]", "");

            return steamApps.Select(app => new
            {
                SteamApp = app,
                Matches = Regex.Matches(Regex.Replace(app.Name?.ToLower(), @"[:;.,_®@-]", ""), $@"\b{Regex.Escape(appName.ToLower())}\b").Count
            })
            .Where(x => x.Matches > 0)
            .OrderByDescending(x => x.Matches)
            .Select(x => x.SteamApp)
            .ToList();
        }
    }
}
