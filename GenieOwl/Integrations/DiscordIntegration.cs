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
        private ServiceProvider _ServiceProvider;

        private readonly IConfiguration _Configuration;

        private readonly DiscordSocketClient _DiscordClient;

        private readonly CommandService _Commands;

        private static readonly DiscordSocketConfig _DiscordConfig = new() { GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent };

        public DiscordIntegration() { }

        public DiscordIntegration(IConfiguration configuration)
        {
            _Configuration = configuration;
            _DiscordClient = new DiscordSocketClient(_DiscordConfig);
            _Commands = new CommandService();
        }

        public async Task StartAsync(ServiceProvider services)
        {
            _ServiceProvider = services;

            await _Commands.AddModulesAsync(Assembly.GetExecutingAssembly(), _ServiceProvider);

            await _DiscordClient.LoginAsync(TokenType.Bot, _Configuration["Discord:Token"]);
            await _DiscordClient.StartAsync();

            _DiscordClient.MessageReceived += this.HandleCommandAsync;
        }

        public async Task StopAsync()
        {
            await _DiscordClient.LogoutAsync();
            await _DiscordClient.StopAsync();
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            int position = 0;
            var userMessage = (SocketUserMessage)arg;

            if (MessageIsUserCommand(userMessage, ref position))
            {
                await _Commands.ExecuteAsync(
                    new SocketCommandContext(_DiscordClient, userMessage),
                    position,
                    _ServiceProvider);
            }
        }

        private bool MessageIsUserCommand(SocketUserMessage userMessage, ref int position)
        {
            return userMessage != null &&
                userMessage.HasStringPrefix(_Configuration["Discord:CommandPrefix"], ref position) &&
                !userMessage.HasMentionPrefix(_DiscordClient.CurrentUser, ref position) &&
                !userMessage.Author.IsBot;
        }
    }
}
