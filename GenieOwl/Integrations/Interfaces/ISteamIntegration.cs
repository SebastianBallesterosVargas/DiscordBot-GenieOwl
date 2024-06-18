namespace GenieOwl.Integrations.Interfaces
{
    using GenieOwl.Integrations.Entities;
    using System.Collections.Generic;

    public interface ISteamIntegration
    {
        /// <summary>
        /// Obtiene el listado de todas las aplicaciones de Steam sin DLCs
        /// </summary>
        /// <returns>Aplicaciones de Steam</returns>
        public List<SteamApp> GetApps();

        /// <summary>
        /// Obtiene el listado de todas las aplicaciones y DLCs de Steam
        /// </summary>
        /// <returns>Aplicaciones y DLCs de Steam</returns>
        public List<SteamApp> GetAppsWithDlcs();

        /// <summary>
        /// Obtiene las noticias por aplicación
        /// </summary>
        /// <param name="appId">Id de la aplicación</param>
        /// <returns>Noticias de la aplicación</returns>
        public List<SteamAppNews> GetNewsByApp(string appId);

        /// <summary>
        /// Obtiene la data de una App de Steam
        /// </summary>
        /// <returns>Datos de la aplicación</returns>
        public SteamApp GetAppAchievements(SteamApp steamApp);
    }
}
