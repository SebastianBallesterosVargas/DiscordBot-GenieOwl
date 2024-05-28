namespace GenieOwl.Integrations.Interfaces
{
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;

    public interface IDiscordIntegration
    {
        public Task StartAsync(ServiceProvider services);

        public Task StopAsync();
    }
}
