namespace GenieOwl.Common.Commands
{
    using Discord.Commands;
    using GenieOwl.Common.Entities;
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Utilities.Messages;
    using Microsoft.Extensions.Configuration;
    
    public class SteamAppCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IConfiguration _Configuration;

        private readonly ISteamService _SteamService;

        public SteamAppCommand(IConfiguration configuration, ISteamService steamService)
        {
            _Configuration = configuration;
            _SteamService = steamService;
        }

        /// <summary>
        /// Busca los logros por aplicación de Steam
        /// </summary>
        /// <param name="gameName">Nombre aplicación de Steam</param>
        /// <returns>Logros de la aplicación de Steam</returns>
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
                bool result = await _SteamService.GetSteamAppsByMatches(gameName, Context);

                if (!result)
                {
                    await ReplyAsync(CustomMessages.GetMessage(MessagesType.UnhandledException));
                }
            }
            catch (Exception ex)
            {
                await ReplyAsync(CustomMessages.GetMessage(MessagesType.GenericError, ex.Message));
            }
        }
    }
}
