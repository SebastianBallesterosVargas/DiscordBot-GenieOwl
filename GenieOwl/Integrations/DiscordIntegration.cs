namespace GenieOwl.Integrations
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using GenieOwl.Integrations.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;
    using System.Reflection;

    public class DiscordIntegration : IDiscordIntegration
    {
        /// <summary>
        /// Servicios de la aplicación
        /// </summary>
        private ServiceProvider _ServiceProvider;

        /// <summary>
        /// Configuración de la aplicación
        /// </summary>
        private readonly IConfiguration _Configuration;

        /// <summary>
        /// Cliente de discord que contiene el contexto de la instancia
        /// </summary>
        private readonly DiscordSocketClient _DiscordClient;

        /// <summary>
        /// Comandos del bot
        /// </summary>
        private readonly CommandService _Commands;

        /// <summary>
        /// Configuración de discord con los permisos disponibles para el bot, debe coincidir con los parámetros del portal administrativo de DiscordDev
        /// </summary>
        private static readonly DiscordSocketConfig _DiscordConfig = new() { GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent };

        public DiscordIntegration(IConfiguration configuration)
        {
            _Configuration = configuration;
            _DiscordClient = new DiscordSocketClient(_DiscordConfig);
            _Commands = new CommandService();
        }

        /// <summary>
        /// Obtiene el contexto de la instancia del DiscordClient
        /// </summary>
        /// <returns>Instancia del DiscordClient</returns>
        public DiscordSocketClient GetDiscordClient()
        {
            return _DiscordClient;
        }

        /// <summary>
        /// Inicializa el Bot de discord
        /// </summary>
        /// <param name="services">Servicios a implementar en la instancia del bot</param>
        public async Task StartAsync(ServiceProvider services)
        {
            _ServiceProvider = services;

            await _Commands.AddModulesAsync(Assembly.GetExecutingAssembly(), _ServiceProvider);

            await _DiscordClient.LoginAsync(TokenType.Bot, _Configuration["Discord:Token"]);
            await _DiscordClient.StartAsync();

            _DiscordClient.MessageReceived += this.HandleCommandAsync;
        }

        /// <summary>
        /// Finaliza el Bot de discord
        /// </summary>
        public async Task StopAsync()
        {
            await _DiscordClient.LogoutAsync();
            await _DiscordClient.StopAsync();
        }

        /// <summary>
        /// Controla los cambios en el chat y ejecuta los comandos enviados
        /// </summary>
        /// <param name="msg">Mensajes del chat</param>
        private async Task HandleCommandAsync(SocketMessage msg)
        {
            int position = 0;
            var userMessage = (SocketUserMessage)msg;

            if (MessageIsCommand(userMessage, ref position))
            {
                await _Commands.ExecuteAsync(
                    new SocketCommandContext(_DiscordClient, userMessage),
                    position,
                    _ServiceProvider);
            }
        }

        /// <summary>
        /// Valida que el mensaje recibido sea un comando
        /// </summary>
        /// <param name="userMessage">Mensaje</param>
        /// <param name="position">Posición asignada por discord a los comandos disponibles</param>
        /// <returns>Boleano que indica si es un comando</returns>
        private bool MessageIsCommand(SocketUserMessage userMessage, ref int position)
        {
            return userMessage != null &&
                userMessage.HasStringPrefix(_Configuration["Discord:CommandPrefix"], ref position) &&
                !userMessage.HasMentionPrefix(_DiscordClient.CurrentUser, ref position);
        }
    }
}
