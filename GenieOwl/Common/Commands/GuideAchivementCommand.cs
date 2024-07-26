namespace GenieOwl.Common.Commands
{
    using Discord;
    using Discord.Commands;
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Integrations.Entities;
    using GenieOwl.Utilities.Messages;
    using GenieOwl.Utilities.Types;
    using Microsoft.Extensions.Configuration;

    public class GuideAchivementCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IDiscordService _DiscordService;

        private readonly ISteamService _SteamService;

        public GuideAchivementCommand(IConfiguration configuration, ISteamService steamService, IDiscordService discordService)
        {
            this._SteamService = steamService;
            this._DiscordService = discordService;
        }

        /// <summary>
        /// Orquesta las respuestas de la búsqueda de logros por aplicación de Steam en inglés
        /// </summary>
        /// <param name="gameName">Nombre aplicación de Steam a búscar</param>
        /// <returns>Respuesta asincrona para el mensaje de Discord</returns>
        [Command("game")]
        [Summary("Search achievements for game in english")]
        public async Task ExecuteAsync([Remainder][Summary("A steam game")] string gameName)
        {
            await Execute(gameName, LenguageType.English);
        }

        /// <summary>
        /// Orquesta las respuestas de la búsqueda de logros por aplicación de Steam en inglés
        /// </summary>
        /// <param name="gameName">Nombre aplicación de Steam a búscar</param>
        /// <returns>Respuesta asincrona para el mensaje de Discord</returns>
        [Command("g")]
        [Summary("Search achievements for game in english. Short command")]
        public async Task ShortExecuteAsync([Remainder][Summary("A steam game")] string gameName)
        {
            await Execute(gameName, LenguageType.English);
        }

        /// <summary>
        /// Orquesta las respuestas de la búsqueda de logros por aplicación de Steam en inglés
        /// </summary>
        /// <param name="gameName">Nombre aplicación de Steam a búscar</param>
        /// <returns>Respuesta asincrona para el mensaje de Discord</returns>
        [Command("game-es")]
        [Summary("Search achievements for game in spanish")]
        public async Task ExecuteSpanishAsync([Remainder][Summary("A steam game")] string gameName)
        {
            await Execute(gameName, LenguageType.Spanish);
        }

        [Command("g-es")]
        [Summary("Search achievements for game in spanish. Short command")]
        public async Task ShortExecuteSpanishAsync([Remainder][Summary("A steam game")] string gameName)
        {
            await Execute(gameName, LenguageType.Spanish);
        }

        private async Task Execute(string gameName, LenguageType lenguage)
        {
            if (string.IsNullOrEmpty(gameName))
            {
                await ReplyAsync(CustomMessages.GetMessage(MessagesType.HelpGameCommand));
                return;
            }

            try
            {
                bool isBotMessage = this.Context.Message.Author.IsBot;

                List<ButtonBuilder> appsButtonsResult = this._SteamService.GetSteamAppsByMatches(gameName, isBotMessage);

                if (appsButtonsResult != null)
                {
                    await this._DiscordService.CreateMessageResponse(this.Context, appsButtonsResult, lenguage, isBotMessage);
                }
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }
    }
}
