namespace GenieOwl.Common.Services
{
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Integrations.Entities;
    using GenieOwl.Integrations.Interfaces;
    using GenieOwl.Utilities.Messages;
    using GenieOwl.Utilities.Types;
    using Microsoft.Extensions.Configuration;

    public class PerplexityService : IPerplexityService
    {
        private static IConfiguration _Configuration;

        private static IPerplexityIntegration _PerplexityIntegration;

        public PerplexityService(IConfiguration configuration, IPerplexityIntegration perplexityIntegration)
        {
            _PerplexityIntegration = perplexityIntegration;
            _Configuration = configuration;
        }

        public async Task<string> GetGuideForAchievement(DiscordButtonData discordButtonData, LenguageType lenguage)
        {
            if (string.IsNullOrEmpty(discordButtonData.AchievementName) || string.IsNullOrEmpty(discordButtonData.Name))
            {
                throw CustomMessages.ResponseMessageEx(MessagesType.GenericError);
            }

            string guideResponse = await _PerplexityIntegration.GetPerplexityChatResponse([discordButtonData.AchievementName, discordButtonData.Name], lenguage);

            bool startIsCommand = !string.IsNullOrEmpty(guideResponse) && guideResponse.Substring(1) == _Configuration["Discord:CommandPrefix"];

            if (startIsCommand)
            {
                guideResponse = guideResponse.Substring(1);
            }

            return guideResponse;
        }
    }
}
