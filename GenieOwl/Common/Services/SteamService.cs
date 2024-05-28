namespace GenieOwl.Common.Services
{
    using Discord.Commands;
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Integrations.Entities;
    using GenieOwl.Integrations.Interfaces;
    using GenieOwl.Utilities.Messages;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;
    using Discord;
    using Newtonsoft.Json;
    using Microsoft.VisualBasic;
    using System.Xml;

    public class SteamService : ISteamService
    {
        private static ISteamIntegration _SteamIntegration;

        private static DiscordService _DiscordService;

        private static List<SteamApp> _SteamApps;

        private static List<SteamApp> _SteamAppsWithDlcs;

        public SteamService(ISteamIntegration steamIntegration)
        {
            _SteamIntegration = steamIntegration;

            _SteamApps = _SteamIntegration.GetApps();
            //this._SteamAppsWithDlcs = this._SteamIntegration.GetAppsWithDlcs();
        }

        public async Task<bool> GetSteamAppsByMatches(string appName, SocketCommandContext context)
        {
            _DiscordService = new DiscordService(context, new OpenAiIntegration());

            //TODO: Ajustar el método para que DiscordService no sea llamado desde acá, sino desde el controlador

            //TODO: Serializar entidades para los botones como custom Id, luego deserializarlas en open ai service para obtener los parametros de SteamApp

            //var entity = new SteamApp { Name = "John", Id = 30 };
            //var serializedEntity = JsonConvert.SerializeObject(entity);
            //var button = new ButtonBuilder()
            //    .WithLabel("My Button")
            //    .WithCustomId(serializedEntity)
            //    .Build();

            //var deserializedEntity = JsonConvert.DeserializeObject<SteamApp>(interaction.Data.CustomId);




            List<SteamApp> appsMatches = GetAppsMatchesByAppName(appName, _SteamApps);
            if (appsMatches.Count == 1)
                return await GetAppAchievementsButtons(appsMatches.FirstOrDefault());

            if (appsMatches.Count > 1)
                return await GetAppsButtons(appsMatches);

            List<SteamApp> appsDlcsMatches = GetAppsMatchesByAppName(appName, _SteamAppsWithDlcs);
            if (appsDlcsMatches.Count == 1)
                return await GetAppAchievementsButtons(appsDlcsMatches.FirstOrDefault());

            if (appsDlcsMatches.Count > 1)
                return await GetAppsButtons(appsDlcsMatches);

            throw CustomMessages.ResponseMessageEx(MessagesType.AppNotFound);
        }

        private static async Task<bool> GetAppsButtons(List<SteamApp> steamApps)
        {
            return await _DiscordService.GetAppsButtons(steamApps);
        }

        private static async Task<bool> GetAppAchievementsButtons(SteamApp steamApp)
        {
            SteamApp appAchievements = _SteamIntegration.GetAppAchievements(steamApp);

            if (appAchievements.Achievements != null && appAchievements.Achievements.Count > 0)
            {
                return await _DiscordService.GetAppAchivementsButtons(appAchievements);
            }

            throw CustomMessages.ResponseMessageEx(MessagesType.AppNotAchivement, steamApp.Name);
        }

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
