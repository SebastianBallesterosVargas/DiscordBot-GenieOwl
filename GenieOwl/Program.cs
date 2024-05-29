namespace GenieOwl
{
    using System.Reflection;
    using GenieOwl.Common.Services;
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Integrations;
    using GenieOwl.Integrations.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using GenieOwl.Integrations.Entities;

    public class Program
    {
        private static void Main() =>
            MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .AddJsonFile("C:\\Users\\sebas\\source\\repos\\GenieOwl\\GenieOwl\\appsettings.json", optional: false)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddScoped<IDiscordIntegration, DiscordIntegration>()
                .AddScoped<ISteamIntegration, SteamIntegration>()
                .AddScoped<IOpenAiIntegration, OpenAiIntegration>()
                .AddScoped<IDiscordService, DiscordService>()
                .AddScoped<ISteamService, SteamService>()
                .AddScoped<IOpenAiService, OpenAiService>()
                .BuildServiceProvider();

            var inter = new OpenAiIntegration(configuration);

            var openAi = new OpenAiService(inter);
            openAi.GetChatResponse(new SteamApp());

            try
            {
                IDiscordIntegration bot = serviceProvider.GetRequiredService<IDiscordIntegration>();

                await bot.StartAsync(serviceProvider);

                Console.WriteLine("Connected to Discord");

                do
                {
                    var keyInfo = Console.ReadKey();

                    if (keyInfo.Key == ConsoleKey.Q)
                    {
                        Console.WriteLine("\nShutting down!");

                        await bot.StopAsync();
                        return;
                    }
                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(-1);
            }
        }
    }
}
