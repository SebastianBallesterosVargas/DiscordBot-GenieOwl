namespace GenieOwl.Integrations
{
    using System;
    using System.Net.Http.Headers;
    using Discord;
    using GenieOwl.Integrations.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class OpenAiIntegration : IOpenAiIntegration
    {
        private static IConfiguration _Configuration;

        public OpenAiIntegration(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        public async Task<string> GetChatResponseByOpenAi(string prompt)
        {
            string guidePrompt = "From now on, you will be a bot named GenieOwl, tasked with providing support for searching guides on achievements for video games. " +
                "Based on the game's name and the achievement, you will need to search for guides to obtain the achievement and explain concisely what needs to be done to achieve it.";

            string game = "Baldur's Gate 3";
            string achievement = "Under Lock and Key";


            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _Configuration["Perplexity:Key"]);

                var requestData = new PerplexyModel()
                {
                    model = "llama-3-sonar-small-32k-online",
                    messages = new List<Message>()
                    {
                        new Message {
                            role = "system",
                            content = guidePrompt
                        },
                        new Message {
                            role = "user",
                            content = String.Format("The game is \"{0}\" and the achievement is \"{1}\"", game, achievement)
                        }
                    }
                };

                var response = await httpClient.PostAsync("https://api.perplexity.ai/chat/completions", new StringContent(JsonConvert.SerializeObject(requestData), System.Text.Encoding.UTF8, "application/json"));


                try
                {
                    if (response.IsSuccessStatusCode)
                        {
                        var result = await response.Content.ReadAsStringAsync();

                        //var result2 = "{\"id\": \"269d1870-a3a5-477f-9c81-0cd39c2ba526\", \"model\": \"llama-3-sonar-small-32k-online\", \"created\": 1716943800, \"usage\": {\"prompt_tokens\": 114, \"completion_tokens\": 169, \"total_tokens\": 283}, \"object\": \"chat.completion\", \"choices\": [{\"index\": 0, \"finish_reason\": \"stop\", \"message\": {\"role\": \"assistant\", \"content\": \"The \\\"Under Lock and Key\\\" achievement in Baldur's Gate 3 requires you to rescue all prisoners from the Moonrise Towers without fighting or anyone turning hostile. Here's a concise guide to help you achieve it:\\n\\n1. **Jump into the oubliette**.\\n2. **Escape the oubliette**.\\n3. **Go to the boat**.\\n4. **Break the boat's chains**.\\n5. **Send characters to break walls**.\\n6. **Break the walls**.\\n7. **Escape**.\\n\\nAdditionally, if you side with the Goblins to get Minthara and raid the Grove, it is achievable without the Tieflings, as confirmed by other players. However, make sure you do not murder the Grove, as that would prevent you from getting the achievement.\"}, \"delta\": {\"role\": \"assistant\", \"content\": \"\"}}]}";

                        var responseData = JsonConvert.DeserializeObject<dynamic>(result);

                        string content = responseData.choices[0].message.content;


                        Console.WriteLine(content);
                    }
                    else
                    {
                        // Maneja el error si la solicitud no fue exitosa
                        //Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return string.Empty;
        }
    }
}

public class PerplexyModel
{
    public string model { get; set; }
    public List<Message> messages { get; set; }
}

public class Message
{
    public string role { get; set; }
    public string content { get; set; }
}
