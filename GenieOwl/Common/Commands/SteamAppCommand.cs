namespace GenieOwl.Common.Commands
{
    using Discord.Commands;
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Integrations.Entities;
    using GenieOwl.Utilities.Messages;
    using Microsoft.Extensions.Configuration;
    
    public class SteamAppCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IConfiguration _Configuration;

        private readonly IDiscordService _DiscordService;

        private readonly ISteamService _SteamService;

        public SteamAppCommand(IConfiguration configuration, ISteamService steamService, IDiscordService discordService)
        {
            this._Configuration = configuration;
            this._SteamService = steamService;
            this._DiscordService = discordService;
        }

        /// <summary>
        /// Orquesta las respuestas de la búsqueda de logros por aplicación de Steam
        /// </summary>
        /// <param name="gameName">Nombre aplicación de Steam a búscar</param>
        /// <returns>Respuesta asincrona para el mensaje de Discord</returns>
        [Command("game")]
        [Summary("Search achievements for game")]
        public async Task ExecuteAsync([Remainder][Summary("A game")] string gameName)
        {
            if (string.IsNullOrEmpty(gameName))
            {
                await ReplyAsync(CustomMessages.GetMessage(MessagesType.HelpGameCommand));
                return;
            }

            try
            {
                bool buttonsResult = false;

                List<SteamApp> appsResult = this._SteamService.GetSteamAppsByMatches(gameName);

                if (appsResult.Count == 1)
                { 
                    buttonsResult = await this._DiscordService.GetAppAchivementsButtons(appsResult.FirstOrDefault(), this.Context);
                }
                else
                {
                    buttonsResult = await this._DiscordService.GetAppsButtons(appsResult, this.Context);
                }
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }
    }
}
