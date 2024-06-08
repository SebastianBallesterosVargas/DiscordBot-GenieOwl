namespace GenieOwl.Integrations
{
    using GenieOwl.Integrations.Entities;
    using GenieOwl.Integrations.Interfaces;
    using GenieOwl.Utilities.Messages;
    using GenieOwl.Utilities.Prompts;
    using Newtonsoft.Json;
    using Microsoft.Extensions.Configuration;
    using System.Net.Http.Headers;
    using GenieOwl.Utilities.Types;

    public class PerplexityIntegration : IPerplexityIntegration
    {
        private static IConfiguration _Configuration;

        private static LenguageType _Lenguage = LenguageType.English;

        public PerplexityIntegration(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        public async Task<string> GetPerplexityChatResponse(string[] promptParameters, LenguageType lenguage)
        {
            _Lenguage = lenguage;

            HttpResponseMessage response = await GetGuidePerplexityResponse(promptParameters);

            string content = await response.Content.ReadAsStringAsync();
            dynamic? responseData = JsonConvert.DeserializeObject<dynamic>(content);

            return responseData?.choices[0].message.content ?? string.Empty;
        }

        private static async Task<HttpResponseMessage> GetGuidePerplexityResponse(string[] promptParameters)
        {
            using var httpClient = new HttpClient();
            
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _Configuration["Perplexity:Key"]);

            PerplexityModel requestData = GetPerplexityModel(promptParameters);

            HttpResponseMessage response = await httpClient.PostAsync(_Configuration["Perplexity:Api"],
                new StringContent(JsonConvert.SerializeObject(requestData), System.Text.Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                throw CustomMessages.ResponseMessageEx(MessagesType.GenericError, $"{response.StatusCode} - {response.ReasonPhrase}");
            }

            return response;
        }

        private static PerplexityModel GetPerplexityModel(string[] promptParameters)
        {
            return new PerplexityModel
            {
                model = _Configuration["Perplexity:Model:Gpt4"],
                messages = new List<PerplexityMessage>
                {
                    new PerplexityMessage
                    {
                        role = "system",
                        content = Prompts.GetPrompt(_Lenguage, PromptType.SystemRole)
                    },
                    new PerplexityMessage
                    {
                        role = "user",
                        content = Prompts.GetPrompt(_Lenguage, PromptType.AchievementGuide, promptParameters)
                    }
                }
            };
        }
    }
}
