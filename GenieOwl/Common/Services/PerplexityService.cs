namespace GenieOwl.Common.Services
{
    using GenieOwl.Common.Entities;
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Integrations.Interfaces;
    using GenieOwl.Utilities.Messages;
    using GenieOwl.Utilities.Types;
    using Microsoft.Extensions.Configuration;

    public class PerplexityService : IPerplexityService
    {
        /// <summary>
        /// Configuración de la app
        /// </summary>
        private static IConfiguration _Configuration;

        /// <summary>
        /// Acceso a la integracion de perplexity para el uso de Gpt
        /// </summary>
        private static IPerplexityIntegration _PerplexityIntegration;

        public PerplexityService(IConfiguration configuration, IPerplexityIntegration perplexityIntegration)
        {
            _PerplexityIntegration = perplexityIntegration;
            _Configuration = configuration;
        }

        /// <summary>
        /// Obtiene la guía para un logro por aplicación
        /// </summary>
        /// <param name="discordButtonData">Objeto con los parámetros de consulta a Perplexity</param>
        /// <param name="lenguage">Idioma de consulta</param>
        /// <returns>Guía del logro por aplicación</returns>
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
