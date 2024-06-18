namespace GenieOwl.Common.Interfaces
{
    using GenieOwl.Common.Entities;
    using GenieOwl.Utilities.Types;

    public interface IPerplexityService
    {
        /// <summary>
        /// Obtiene la guía para un logro por aplicación
        /// </summary>
        /// <param name="discordButtonData">Objeto con los parámetros de consulta a Perplexity</param>
        /// <param name="lenguage">Idioma de consulta</param>
        /// <returns>Guía del logro por aplicación</returns>
        public Task<string> GetGuideForAchievement(DiscordButtonData discordButtonData, LenguageType lenguage);
    }
}
