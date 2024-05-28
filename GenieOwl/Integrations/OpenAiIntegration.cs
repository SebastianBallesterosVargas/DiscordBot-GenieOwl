namespace GenieOwl.Integrations
{
    using System;
    using GenieOwl.Integrations.Interfaces;
    using Microsoft.Extensions.Configuration;
    using OpenAI_API;
    using OpenAI_API.Chat;
    using OpenAI_API.Models;

    public class OpenAiIntegration : IOpenAiIntegration
    {
        private static IConfiguration _Configuration;

        private readonly IOpenAIAPI _OpenAiApi;

        public OpenAiIntegration(IConfiguration configuration)
        {
            _Configuration = configuration;
            _OpenAiApi = new OpenAIAPI(_Configuration["OpenAI:Key"]);
        }

        public async Task<string> GetChatResponseByOpenAi(string prompt)
        {
            //Model model = new Model("gpt-3.5-turbo");
            //var messages = new List<ChatMessage>
            //{
            //    new ChatMessage()
            //    {
            //        TextContent = message,
            //    }
            //};


            var chat = _OpenAiApi.Chat.CreateConversation();
            chat.Model = Model.GPT4_Turbo;

            ChatResult result = await _OpenAiApi.Chat.CreateChatCompletionAsync(prompt);
            Console.WriteLine(result);

            return result.Choices?.FirstOrDefault()?.ToString() ?? string.Empty;
        }
    }
}
