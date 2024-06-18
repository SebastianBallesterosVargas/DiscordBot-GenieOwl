namespace GenieOwl.Common.Interfaces
{
    using Discord;

    public interface ISteamService
    {
        /// <summary>
        /// Obtiene las aplicaciones de Steam que coincidan con la búsqueda dentro de SteamAPI y las retorna en formato de botón
        /// </summary>
        /// <param name="appName">Nombre de la búsqueda</param>
        /// <returns>Constructor de los botones a responder</returns>
        public List<ButtonBuilder> GetSteamAppsByMatches(string appName, bool isBotMessage);
    }
}
