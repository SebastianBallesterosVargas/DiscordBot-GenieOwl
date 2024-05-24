namespace GenieOwl.Common.Commands
{
    using Discord;
    using Discord.Commands;
    using GenieOwl.Common.Services;
    using GenieOwl.Integrations.Entities;
    using Microsoft.Extensions.Configuration;
    using System.Net.Http.Headers;
    
    public class SteamAppCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IConfiguration _Configuration;

        private readonly HttpClient _httpClient;

        public SteamAppCommand(IConfiguration configuration)
        {
            _Configuration = configuration;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "DiscordBot");
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
                await ReplyAsync("Usage: !game <game name>");
                return;
            }

            try
            {
                SteamService steamService = new(_Configuration);
                    
                List<SteamApp>? steamAppsResponse = steamService.GetSteamAppsByMatches(gameName);

                Console.WriteLine(steamAppsResponse);

                //gameName = Uri.EscapeDataString(gameName);

                //var response = await _httpClient.GetStringAsync($"http://api.urbandictionary.com/v0/define?term={gameName}");

                if (steamAppsResponse == null || steamAppsResponse.Count == 0)
                {
                    await ReplyAsync($"Nothing found for {gameName}");
                    return;
                }
                else
                {
                    var appsFounded = string.Join(", ", steamAppsResponse.Select(aplicacion => aplicacion.Name));

                    await ReplyAsync($"Cuál de los siguientes juegos quieres consultar?");
                    await ReplyAsync(appsFounded);
                    //await ReplyAsync(new DiscordButtonComponent(ButtonStyle.Primary, "my_button_id", "This is a button!"));
                }
            }
            //catch (HttpRequestException)
            //{
            //    await ReplyAsync("Error making the request to Urban Dictionary API");
            //}
            catch (Exception ex)
            {
                await ReplyAsync($"An error occurred: {ex.Message}");
            }
        }
    }

    public class UrbanDictionaryResponse
    {
        public List<UrbanDictionaryItem>? List { get; set; }
    }

    public class UrbanDictionaryItem
    {
        public string? Definition { get; set; }
    }
}
