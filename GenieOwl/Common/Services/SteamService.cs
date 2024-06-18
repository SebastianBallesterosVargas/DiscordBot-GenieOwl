namespace GenieOwl.Common.Services
{
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Integrations.Entities;
    using GenieOwl.Integrations.Interfaces;
    using GenieOwl.Utilities.Messages;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;
    using GenieOwl.Utilities.Types;
    using Discord;
    using GenieOwl.Common.Entities;
    using Newtonsoft.Json;
    using System.Drawing;

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

        /// <summary>
        /// Valor en píxeles de un espacio en blanco
        /// </summary>
        private const int _BlankSpaceForPixelWidth = 11;

        /// <summary>
        /// Valor en píxeles máximo por texto de botón
        /// </summary>
        private const int _MaxLongForPixelWidth = 187;

        public SteamService(ISteamIntegration steamIntegration)
        {
            _SteamIntegration = steamIntegration;

            _SteamApps = _SteamIntegration.GetApps();
            _SteamAppsWithDlcs = _SteamIntegration.GetAppsWithDlcs();
        }

        /// <summary>
        /// Obtiene las aplicaciones de Steam que coincidan con la búsqueda dentro de SteamAPI
        /// </summary>
        /// <param name="appName">Nombre de la búsqueda</param>
        /// <returns>Aplicaciones de Steam</returns>
        public List<ButtonBuilder> GetSteamAppsByMatches(string appName, bool isBotMessage)
        {
            List<SteamApp> appsMatches = GetAppsMatchesByAppName(appName, _SteamApps, isBotMessage);
            if (appsMatches.Count == 1)
                return GetAppAchivementsButtons(appsMatches.FirstOrDefault());

            if (appsMatches.Count > 1)
                return GetAppsButtons(appsMatches);

            List<SteamApp> appsDlcsMatches = GetAppsMatchesByAppName(appName, _SteamAppsWithDlcs, isBotMessage);
            if (appsDlcsMatches.Count == 1)
                return GetAppAchivementsButtons(appsDlcsMatches.FirstOrDefault());

            if (appsDlcsMatches.Count > 1)
                return GetAppsButtons(appsDlcsMatches);

            throw CustomMessages.ResponseMessageEx(MessagesType.AppNotFound);
        }

        /// <summary>
        /// Obtiene el contructor de botones para las aplicaciones de Steam
        /// </summary>
        /// <param name="steamApps">Aplicaciones de Steam</param>
        /// <returns>Aplicaciones en formato de botones</returns>
        private static List<ButtonBuilder> GetAppsButtons(List<SteamApp> steamApps)
        {
            var buttonsBuilder = new List<ButtonBuilder>();

            foreach (SteamApp app in steamApps)
            {
                var interactiveButton = new ButtonBuilder()
                    .WithLabel(GetAppDisplayName(app.Name))
                    .WithStyle(ButtonStyle.Secondary)
                    .WithCustomId(GetSerializedId(new DiscordButtonData
                    {
                        Id = app.Id,
                        Name = app.Name
                    }));

                buttonsBuilder.Add(interactiveButton);
            }

            return buttonsBuilder;
        }

        /// <summary>
        /// Obtiene el constructor de botones para los logros de una aplicación de Steam
        /// </summary>
        /// <param name="steamApp">Aplicación de Steam a obtener logros</param>
        /// <returns>Logros en formato de botones</returns>
        private static List<ButtonBuilder> GetAppAchivementsButtons(SteamApp steamApp)
        {
            var buttonsBuilder = new List<ButtonBuilder>();

            SteamApp appAchievements = _SteamIntegration.GetAppAchievements(steamApp);

            if (appAchievements.Achievements == null || appAchievements.Achievements.Count == 0)
            {
                throw CustomMessages.ResponseMessageEx(MessagesType.AppNotAchivement, steamApp.Name);
            }

            foreach (SteamAppAchievement appAchievement in appAchievements.Achievements.OrderBy(app => !app.Hidden))
            {
                var interactiveButton = new ButtonBuilder()
                    .WithLabel(GetAppDisplayName(appAchievement.DisplayName, appAchievement.Hidden))
                    .WithStyle(ButtonStyle.Secondary)
                    .WithCustomId(GetSerializedId(new DiscordButtonData
                    {
                        Id = steamApp.Id,
                        Name = steamApp.Name,
                        AchievementName = appAchievement.DisplayName
                    }));

                buttonsBuilder.Add(interactiveButton);
            }

            return buttonsBuilder;
        }

        /// <summary>
        /// Busca en las aplicaciones de Steam que coincidan con el appName
        /// </summary>
        /// <param name="appName">Nombre de la aplicación buscada</param>
        /// <param name="steamApps">Aplicaciones de Steam a buscar</param>
        /// <returns>Coincidencias por nombre de aplicación</returns>
        private static List<SteamApp> GetAppsMatchesByAppName(string appName, List<SteamApp> steamApps, bool isBotMessage)
        {
            if (steamApps == null || steamApps.Count == 0)
            {
                throw CustomMessages.ResponseMessageEx(MessagesType.NotSteamApps);
            }

            if (isBotMessage)
            {
                return steamApps.Where(app => app.Name == appName).ToList();
            }
            else
            {
                string cleanedAppName = Regex.Replace(appName.ToLower(), @"[':;.,_®@-]", "");

                return steamApps.Select(app => new
                {
                    SteamApp = app,
                    Matches = Regex.Matches(Regex.Replace(app.Name?.ToLower(), @"[':;.,_®@-]", ""), $@"\b{Regex.Escape(cleanedAppName)}\b").Count
                })
                .Where(x => x.Matches > 0)
                .OrderByDescending(x => x.Matches)
                .Select(x => x.SteamApp)
                .ToList();
            }
        }

        /// <summary>
        /// Obtiene el nombre del botón de aplicación con espaciado en blanco para simular el tamaño de los botones por pixel
        /// </summary>
        /// <param name="displayName">Nombre del botón</param>
        /// <param name="isHidden">Boleano que indica si es un logro oculto</param>
        /// <returns>Retorna el nombre con espaciado por pixel</returns>
        private static string GetAppDisplayName(string displayName, bool isHidden = false)
        {
            if (!string.IsNullOrEmpty(displayName))
            {
                char blankSpace = '\u2800';

                displayName += isHidden ? $" {CustomEmotes.GetEmote(EmoteType.HiddenEye)}" : string.Empty;

                int inputLength = MeasureStringWidth(displayName);
                int spacesToAdd = _MaxLongForPixelWidth - inputLength;

                if (inputLength >= _MaxLongForPixelWidth || spacesToAdd <= 0)
                {
                    return displayName;
                }

                return displayName + new string(blankSpace, (spacesToAdd / _BlankSpaceForPixelWidth));
            }

            return displayName;
        }

        /// <summary>
        /// Retorna el tamaño del texto actual por pixel
        /// </summary>
        /// <param name="appName">Nombre a medir</param>
        /// <returns>Valor del ancho del texto en pixeles</returns>
        private static int MeasureStringWidth(string appName)
        {
            Font fontSize = SystemFonts.DefaultFont;

            using (var bmp = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(bmp))
            {
                SizeF size = g.MeasureString(appName, fontSize);
                return (int)size.Width;
            }
        }

        /// <summary>
        /// Serializa una entidad
        /// </summary>
        /// <typeparam name="T">Entidad de tipo a serializar</typeparam>
        /// <param name="entity">Entidad a serializar</param>
        /// <returns>Entidad serializada</returns>
        private static string GetSerializedId<T>(T entity)
        {
            return JsonConvert.SerializeObject(entity);
        }
    }
}
