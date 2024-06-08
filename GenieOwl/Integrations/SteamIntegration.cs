namespace GenieOwl.Integrations
{
    using GenieOwl.Integrations.Entities;
    using GenieOwl.Integrations.Interfaces;
    using Microsoft.Extensions.Configuration;
    using SteamKit2;
    using System;
    using System.Collections.Generic;

    public class SteamIntegration : ISteamIntegration
    {
        /// <summary>
        /// Configuración de la aplicación
        /// </summary>
        private static IConfiguration _Configuration;

        /// <summary>
        /// Maximo de resultados permitidos por el método GetAppList de Steam API
        /// </summary>
        private static readonly int _MaxResultsAvaibleForSteamApi = 50000;

        public SteamIntegration(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        /// <summary>
        /// Obtiene el listado de todas las aplicaciones de Steam sin DLCs
        /// </summary>
        /// <returns>Aplicaciones de Steam</returns>
        public List<SteamApp> GetApps()
        {
            var steamApps = new List<SteamApp>();

            var arguments = new Dictionary<string, object?>
            {
                ["include_dlc"] = false,
                ["max_results"] = _MaxResultsAvaibleForSteamApi
            };

            KeyValue steamAppsResponse = SteamKitCall("IStoreService", "GetAppList", arguments, 1);

            foreach (KeyValue app in steamAppsResponse["apps"].Children)
            {
                var steamApp = new SteamApp
                {
                    Id = app["appid"].AsInteger(),
                    Name = app["name"].AsString(),
                    LastUpdate = UnixTimeStampToDateTime(app["last_modified"].AsLong()),
                    PriceChangeValue = app["price_change_number"].AsInteger()
                };

                steamApps.Add(steamApp);
            }

            return steamApps;
        }

        /// <summary>
        /// Obtiene el listado de todas las aplicaciones y DLCs de Steam
        /// </summary>
        /// <returns>Aplicaciones y DLCs de Steam</returns>
        public List<SteamApp> GetAppsWithDlcs()
        {
            var steamApps = new List<SteamApp>();

            KeyValue steamAppsResponse = SteamKitCall("ISteamApps", "GetAppList", null, 2);

            foreach (KeyValue app in steamAppsResponse["apps"].Children)
            {
                var steamApp = new SteamApp
                {
                    Id = app["appid"].AsInteger(),
                    Name = app["name"].AsString()
                };

                steamApps.Add(steamApp);
            }

            return steamApps;
        }

        /// <summary>
        /// Obtiene la data de una App de Steam
        /// </summary>
        /// <returns>Datos de la aplicación</returns>
        public SteamApp GetAppAchievements(SteamApp steamApp)
        {
            var arguments = new Dictionary<string, object?> { ["appid"] = steamApp.Id.ToString() };

            KeyValue appSchemaResponse = SteamKitCall("ISteamUserStats", "GetSchemaForGame", arguments, 2);

            return new SteamApp()
            {
                Id = steamApp.Id,
                Name = steamApp.Name,
                AppVersion = appSchemaResponse["gameVersion"].AsInteger(),
                Achievements = GetAchievementsForApp(appSchemaResponse)
            };
        }

        /// <summary>
        /// Obtiene las noticias por aplicación
        /// </summary>
        /// <param name="appId">Id de la aplicación</param>
        /// <returns>Noticias de la aplicación</returns>
        public List<SteamAppNews> GetNewsByApp(string appId)
        {
            var appNews = new List<SteamAppNews>();

            var arguments = new Dictionary<string, object?> { ["appid"] = appId };

            KeyValue appNewsResponse = SteamKitCall("ISteamNews", "GetNewsForApp", arguments);

            foreach (KeyValue news in appNewsResponse["newsitems"]["newsitem"].Children)
            {
                var steamAppNews = new SteamAppNews
                {
                    Gid = news["gid"].AsString(),
                    Title = news["title"].AsString(),
                    Url = news["url"].AsString(),
                    Contents = news["contents"].AsString(),
                    IsExternalUrl = news["is_external_url"].AsBoolean(),
                    AppId = news["appid"].AsInteger(),
                    Date = UnixTimeStampToDateTime(news["date"].AsLong())
                };

                appNews.Add(steamAppNews);
            }

            return appNews;
        }

        /// <summary>
        /// Obtiene los logros de una aplicación de Steam
        /// </summary>
        /// <param name="appSchema">Respuesta con detalle de logros por aplicación</param>
        /// <returns>Retorna el mapeo de los logros de la aplicación</returns>
        private static List<SteamAppAchievement> GetAchievementsForApp(KeyValue appSchema)
        {
            var appAchievements = new List<SteamAppAchievement>();

            foreach (KeyValue achievement in appSchema["availableGameStats"]["achievements"].Children)
            {
                var appAchievement = new SteamAppAchievement
                {
                    Id = achievement["name"].Value,
                    DisplayName = achievement["displayName"].Value,
                    Description = achievement["description"].Value,
                    Icon = achievement["icon"].Value,
                    IconGray = achievement["iconGray"].Value,
                    Hidden = achievement["hidden"].AsBoolean()
                };

                appAchievements.Add(appAchievement);
            }

            return appAchievements;
        }

        /// <summary>
        /// Añade los segundos de la marca de tiempo al inicio de UNIX para obtener la fecha y hora
        /// </summary>
        /// <param name="unixDate">Marca de tiempo en formato UNIX</param>
        /// <returns>Fecha y hora en tipo DateTime</returns>
        private static DateTime? UnixTimeStampToDateTime(long unixDate)
        {
            if (unixDate > 0)
            {
                DateTime startUnix = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                return startUnix.AddSeconds(unixDate).ToLocalTime();
            }
            return null;
        }

        /// <summary>
        /// Orquesta el llamado al método Call de la librería SteamKit2
        /// </summary>
        /// <param name="iface">Interfaz del consumo</param>
        /// <param name="func">Función a consumir</param>
        /// <param name="arguments">Argumentos de la consulta</param>
        /// <param name="apiVersion">Versión del Api a consumir</param>
        /// <returns></returns>
        private static KeyValue SteamKitCall(string iface, string func, Dictionary<string, object?>? arguments = null, int apiVersion = 1)
        {
            using WebAPI.Interface steamInterface = WebAPI.GetInterface(iface, _Configuration["Steam:Key"]);
            return steamInterface.Call(func, apiVersion, arguments);
        }
    }
}
