namespace GenieOwl.Integrations.Interfaces
{
    using Discord.WebSocket;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;

    public interface IDiscordIntegration
    {
        /// <summary>
        /// Obtiene el contexto de la instancia del DiscordClient
        /// </summary>
        /// <returns>Instancia del DiscordClient</returns>
        public DiscordSocketClient GetDiscordClient();

        /// <summary>
        /// Inicializa el Bot de discord
        /// </summary>
        /// <param name="services">Servicios a implementar en la instancia del bot</param>
        public Task StartAsync(ServiceProvider services);

        /// <summary>
        /// Finaliza el Bot de discord
        /// </summary>
        public Task StopAsync();
    }
}
