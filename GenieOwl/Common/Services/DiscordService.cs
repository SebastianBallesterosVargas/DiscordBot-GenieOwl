namespace GenieOwl.Common.Services
{
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Integrations;
    using GenieOwl.Integrations.Entities;
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;

    public class DiscordService : IDiscordService
    {
        private static IConfiguration _Configuration;

        private readonly OpenAiIntegration OpenAiIntegration;

        private readonly SteamIntegration SteamIntegration;

        public DiscordService(IConfiguration configuration)
        {
            _Configuration = configuration;

            SteamIntegration = new SteamIntegration(_Configuration);
            OpenAiIntegration = new OpenAiIntegration(_Configuration);
        }

        //public SteamApp GetSteamApp(string appName)
        //{
        //    //_SteamAppsDlcs = _SteamIntegration.GetAppsWithDlcs();
        //    List<SteamApp> steamApps = SteamIntegration.GetApps();

        //    return SteamIntegration.GetAppAchievements(steamApps[50]);
        //    //List<SteamAppNews> result2 = _SteamIntegration.GetNewsByApp("292030");
        //}

        public void UseOpenAi()
        {
            OpenAiIntegration.GetChatResponseByOpenAi("Buenas tardes!");
        }
    }
}
