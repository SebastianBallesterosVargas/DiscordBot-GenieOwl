namespace GenieOwl
{
    using System.Reflection;
    using GenieOwl.Integrations.Interfaces;
    using GenieOwl.Integrations;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Common.Services;

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
                .AddScoped<IDiscordService, DiscordService>()
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
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Environment.Exit(-1);
            }
        }
    }
}
