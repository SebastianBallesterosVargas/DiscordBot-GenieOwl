namespace GenieOwl.Common.Services
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using GenieOwl.Common.Entities;
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Integrations.Interfaces;
    using GenieOwl.Utilities.Messages;
    using GenieOwl.Utilities.Types;
    using Newtonsoft.Json;

    public class DiscordService : IDiscordService
    {
        /// <summary>
        /// Mensaje incial para las opciones de usuario
        /// </summary>
        private static IMessage _InstanceUserMessage;

        /// <summary>
        /// Servicio de perplexity para la respuesta final del bot
        /// </summary>
        private static IPerplexityService _PerplexityService;

        /// <summary>
        /// Idioma de la respuesta
        /// </summary>
        private static LenguageType _Lenguage = LenguageType.English;

        /// <summary>
        /// Botones con las opciones a ejecutar
        /// </summary>
        private static List<ButtonBuilder> _ButtonsBuilder;

        /// <summary>
        /// Páginado de los botones de opción
        /// </summary>
        private static int _Page = 0;

        /// <summary>
        /// Máximo de botones por página
        /// </summary>
        private const int _MaxCountForPage = 20;

        /// <summary>
        /// Id botón página previa
        /// </summary>
        private const string _IdPreviousPage = "previous_page";

        /// <summary>
        /// Id botón página siguiente
        /// </summary>
        private const string _IdNextPage = "next_page";

        /// <summary>
        /// Mensaje página anterior
        /// </summary>
        private static readonly string _ArrowLeftMessage = $"{CustomEmotes.GetEmote(EmoteType.LeftArrow)} {CustomMessages.GetMessage(MessagesType.PreviousPage)}";

        /// <summary>
        /// Mensaje página siguiente
        /// </summary>
        private static readonly string _ArrowRightMessage = $"{CustomEmotes.GetEmote(EmoteType.RightArrow)} {CustomMessages.GetMessage(MessagesType.NextPage)}";
        
        public DiscordService(IPerplexityService perplexityService, IDiscordIntegration discordIntegration)
        {
            _PerplexityService = perplexityService;
            discordIntegration.GetDiscordClient().InteractionCreated += HandleButtonEvent;
        }

        /// <summary>
        /// Crea un mensaje de respuesta, paginando los botones de selección de apps y logros
        /// </summary>
        /// <param name="context">Contexto de discord para la generación de mensajes</param>
        /// <param name="buttons">Botones con opciones</param>
        /// <param name="lenguage">Lenguaje de respuesta para la guía</param>
        /// <param name="isBotMessage">Indica si el mensaje es del bot o un usuario</param>
        /// <returns>Tarea con ejecucion del emensaje</returns>
        public async Task CreateMessageResponse(SocketCommandContext context, List<ButtonBuilder> buttons, LenguageType lenguage, bool isBotMessage)
        {
            InitializeComponentValues(buttons, lenguage);

            _InstanceUserMessage = await context.Channel.SendMessageAsync(CustomMessages.GetMessage(isBotMessage ? MessagesType.AchievementsFound : MessagesType.AppsFound));

            await ((IUserMessage)_InstanceUserMessage).ModifyAsync(msg =>
            {
                msg.Components = GetButtonComponents().Build();
            });
        }

        /// <summary>
        /// Inicializa los parámetros requeridos para el proceso de creación de mensajes
        /// </summary>
        /// <param name="buttons">Botones de elección del mensaje</param>
        /// <param name="lenguage">Lenguaje de respuesta para la guía</param>
        private static void InitializeComponentValues(List<ButtonBuilder> buttons, LenguageType lenguage)
        {
            _ButtonsBuilder = buttons;
            _Lenguage = lenguage;
            _Page = 0;
        }

        /// <summary>
        /// Evento de selección para los botones del mensaje
        /// </summary>
        /// <param name="interaction">Evento que se recibe al seleccionar un botón, contiene la información del botón seleccionado</param>
        /// <returns>Tarea con respuesta del bot</returns>
        private async Task HandleButtonEvent(SocketInteraction interaction)
        {
            if (interaction is not SocketMessageComponent buttonInteraction)
                return;

            if (buttonInteraction.Message.Id != _InstanceUserMessage.Id)
                return;

            switch (buttonInteraction.Data.CustomId)
            {
                case _IdPreviousPage:
                    _Page--;
                    break;
                case _IdNextPage:
                    _Page++;
                    break;
                default:
                    await HandleButtonOption(buttonInteraction);
                    break;
            }

            await ((IUserMessage)_InstanceUserMessage).ModifyAsync(msg =>
            {
                msg.Components = GetButtonComponents().Build();
            });

            await buttonInteraction.RespondAsync();
        }

        /// <summary>
        /// Retorna un nuevo componente con los botones páginados por menos de la máxima cantidad permitida por Discord (Discord permite un máximo de 25 botones por mensaje)
        /// </summary>
        /// <returns>Componente con el mensaje</returns>
        private static ComponentBuilder GetButtonComponents()
        {
            var componentBuilder = new ComponentBuilder();

            for (int i = _MaxCountForPage * _Page; i < _ButtonsBuilder.Count && i < _MaxCountForPage * (_Page + 1); i++)
            {
                componentBuilder.WithButton(_ButtonsBuilder[i]);
            }

            if (_Page > 0)
                componentBuilder.WithButton(_ArrowLeftMessage, _IdPreviousPage, ButtonStyle.Primary);

            if (_MaxCountForPage * (_Page + 1) < _ButtonsBuilder.Count)
                componentBuilder.WithButton(_ArrowRightMessage, _IdNextPage, ButtonStyle.Primary);

            return componentBuilder;
        }

        /// <summary>
        /// Controla el tipo de opción seleccionada.
        /// En caso de ser un botón de selección de app ejecuta la búsqueda de guías y en caso de ser un botón de guía llama al servicio de perplexity para generar la guía
        /// </summary>
        /// <param name="buttonInteraction">Contiene el evento de interacción con la información del botón seleccionado</param>
        /// <returns>Tarea con respuesta del bot</returns>
        private static async Task HandleButtonOption(SocketMessageComponent buttonInteraction)
        {
            if (buttonInteraction != null && buttonInteraction.Data.CustomId != null)
            {
                DiscordButtonData? discordButtonData = JsonConvert.DeserializeObject<DiscordButtonData>(buttonInteraction.Data.CustomId);

                if (discordButtonData == null)
                {
                    throw CustomMessages.ResponseMessageEx(MessagesType.PerplexityError);
                }

                if (string.IsNullOrEmpty(discordButtonData?.AchievementName))
                {
                    if (_Lenguage == LenguageType.Spanish)
                    {
                        await buttonInteraction.RespondAsync($"!game-es {discordButtonData?.Name}");
                    }
                    else
                    {
                        await buttonInteraction.RespondAsync($"!game {discordButtonData?.Name}");
                    }
                }

                if (!string.IsNullOrEmpty(discordButtonData?.AchievementName))
                {
                    await buttonInteraction.RespondAsync(CustomMessages.GetMessage(MessagesType.GetMomment));

                    string guideMessage = await _PerplexityService.GetGuideForAchievement(discordButtonData, _Lenguage);

                    await buttonInteraction.Channel.SendMessageAsync(guideMessage);
                }
            }
        }
    }
}
