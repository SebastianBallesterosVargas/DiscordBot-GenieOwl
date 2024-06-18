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
        /// <summary>
        /// Configuración de la aplicación
        /// </summary>
        private static IConfiguration _Configuration;

        /// <summary>
        /// Lenguaje de consumo a la API
        /// </summary>
        private static LenguageType _Lenguage = LenguageType.English;

        public PerplexityIntegration(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        /// <summary>
        /// Orquesta el consumo a la API de Perplexity para obtener una respuesta de Gpt con acceso a web
        /// </summary>
        /// <param name="promptParameters">Parámetros de búsqueda, contienen nombre y logro de la app buscada</param>
        /// <param name="lenguage">Lenguaje del prompt en el cual se obtendrá la respuesta</param>
        /// <returns>Respuesta con guía del logro para la app buscada</returns>
        public async Task<string> GetPerplexityChatResponse(string[] promptParameters, LenguageType lenguage)
        {
            _Lenguage = lenguage;

            HttpResponseMessage response = await GetGuidePerplexityResponse(promptParameters);

            string content = await response.Content.ReadAsStringAsync();
            dynamic? responseData = JsonConvert.DeserializeObject<dynamic>(content);

            return responseData?.choices[0].message.content ?? string.Empty;
        }

        /// <summary>
        /// Consumo Http a la API de perplexity
        /// </summary>
        /// <param name="promptParameters">Parámetros de búsqueda, contienen nombre y logro de la app buscada</param>
        /// <returns>Respuesta Http de la API</returns>
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

        /// <summary>
        /// Obtiene el modelo de consumo de la API con los parámetros de consulta
        /// </summary>
        /// <param name="promptParameters">Parámetros de búsqueda, contienen nombre y logro de la app buscada</param>
        /// <returns>Modelo con parámetros de consulta para la API</returns>
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
