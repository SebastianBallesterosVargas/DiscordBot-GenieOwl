namespace GenieOwl.Integrations.Interfaces
{
    using Discord.WebSocket;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;

    public interface IDiscordIntegration
    {
        public DiscordSocketClient GetDiscordClient();

        public Task StartAsync(ServiceProvider services);

        public Task StopAsync();
    }
}
