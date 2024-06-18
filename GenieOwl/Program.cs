namespace GenieOwl
{
    using System.Reflection;
    using GenieOwl.Common.Services;
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Integrations;
    using GenieOwl.Integrations.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        private static void Main() =>
            MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .AddJsonFile("[Your appsettings.json route]", optional: false)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddScoped<IDiscordIntegration, DiscordIntegration>()
                .AddScoped<ISteamIntegration, SteamIntegration>()
                .AddScoped<IPerplexityIntegration, PerplexityIntegration>()
                .AddScoped<IDiscordService, DiscordService>()
                .AddScoped<ISteamService, SteamService>()
                .AddScoped<IPerplexityService, PerplexityService>()
                .BuildServiceProvider();

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
