namespace GenieOwl.Common.Interfaces
{
    using Discord;
    using Discord.Commands;
    using GenieOwl.Utilities.Types;

    public interface IDiscordService
    {
        /// <summary>
        /// Crea un mensaje de respuesta, paginando los botones de selección de apps y logros
        /// </summary>
        /// <param name="context">Contexto de discord para la generación de mensajes</param>
        /// <param name="buttons">Botones con opciones</param>
        /// <param name="lenguage">Lenguaje de respuesta para la guía</param>
        /// <param name="isBotMessage">Indica si el mensaje es del bot o un usuario</param>
        /// <returns>Tarea con ejecucion del emensaje</returns>
        public Task CreateMessageResponse(SocketCommandContext context, List<ButtonBuilder> buttons, LenguageType lenguage, bool isBotMessage);
    }
}
