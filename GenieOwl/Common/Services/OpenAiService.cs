namespace GenieOwl.Common.Services
{
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Integrations.Interfaces;

    public class OpenAiService : IOpenAiService
    {
        private static IOpenAiIntegration OpenAiIntegration;

        public OpenAiService(IOpenAiIntegration openAiIntegration)
        {
            OpenAiIntegration = openAiIntegration;
        }

        public async Task GetChatResponse(string prompt)
        {
            await OpenAiIntegration.GetChatResponseByOpenAi(prompt);
        }
    }
}
