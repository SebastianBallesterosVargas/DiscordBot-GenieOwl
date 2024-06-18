namespace GenieOwl.Integrations.Interfaces
{
    using GenieOwl.Utilities.Types;
    
    public interface IPerplexityIntegration
    {
        /// <summary>
        /// Consume la API de Perplexity para obtener una respuesta de Gpt con acceso a web
        /// </summary>
        /// <param name="promptParameters">Parámetros de búsqueda, contienen nombre y logro de la app buscada</param>
        /// <param name="lenguage">Lenguaje del prompt en el cual se obtendrá la respuesta</param>
        /// <returns>Respuesta con guía del logro para la app buscada</returns>
        public Task<string> GetPerplexityChatResponse(string[] promptParameters, LenguageType lenguage);
    }
}
