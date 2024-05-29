namespace GenieOwl.Common.Services
{
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Integrations.Entities;
    using GenieOwl.Integrations.Interfaces;

    public class OpenAiService : IOpenAiService
    {
        private static IOpenAiIntegration OpenAiIntegration;

        public OpenAiService(IOpenAiIntegration openAiIntegration)
        {
            OpenAiIntegration = openAiIntegration;
        }

        public async Task<string> GetChatResponse(SteamApp steamApp)
        {
            string prompt = "Hay alguna versión de la API de Open Ai con la que puedas acceder a internet?";
            string prompt2 = "Puedes acceder a internet? \n Qué versión de Open Ai estas usando?";


            var openAiRespose = await OpenAiIntegration.GetChatResponseByOpenAi(prompt);
            //var openAiRespose2 = await OpenAiIntegration.GetChatResponseByOpenAi(prompt2);
            return openAiRespose.ToString();
        }

        public async Task<string> GetChatResponse(SteamAppAchievement appAchievements)
        {
            var openAiRespose = await OpenAiIntegration.GetChatResponseByOpenAi(appAchievements.ToString());
            return openAiRespose.ToString();
        }
    }
}
