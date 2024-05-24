namespace GenieOwl.Integrations.Interfaces
{
    using Discord.WebSocket;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;

    public interface IDiscordIntegration
    {
        public Task StartAsync(ServiceProvider services);

        public Task StopAsync();

        public void AddButtonExecutedToDiscordClient(Func<SocketMessageComponent, Task> actionButton);
    }
}
