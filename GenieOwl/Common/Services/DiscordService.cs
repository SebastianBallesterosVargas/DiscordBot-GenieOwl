namespace GenieOwl.Common.Services
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Integrations.Entities;
    using GenieOwl.Utilities;
    using GenieOwl.Utilities.Messages;
    using System.Drawing;

    public class DiscordService : IDiscordService
    {
        private static IMessage _InstanceUserMessage;

        private static IOpenAiService _OpenAiService;

        private static SocketCommandContext _Context;

        private static List<ButtonBuilder> _ButtonsBuilder;
        
        private static string _AppName;

        private static int _Page = 0;

        private static bool _MessageWasSent = false;

        private static readonly string _ArrowLeftMessage = $"{CustomMessages.GetEmote(EmoteType.LeftArrow)} {CustomMessages.GetMessage(MessagesType.PreviousPage)}";

        private static readonly string _ArrowRightMessage = $"{CustomMessages.GetEmote(EmoteType.RightArrow)} {CustomMessages.GetMessage(MessagesType.NextPage)}";

        private const string _IdPreviousPage = "previous_page";

        private const string _IdNextPage = "next_page";

        private const int _BlankSpaceForPixelWidth = 11;
        
        private const int _MaxLongForPixelWidth = 187;

        private const int _MaxCountForPage = 20;

        public DiscordService(SocketCommandContext context, IOpenAiService openAiService)
        {
            _Context = context;
            _OpenAiService = openAiService;
        }

        public async Task<bool> GetAppsButtons(List<SteamApp> steamApps)
        {
            ResetComponentValues();

            foreach (SteamApp app in steamApps)
            {
                var interactiveButton = new ButtonBuilder()
                    .WithLabel(GetAppDisplayName(app.Name))
                    .WithStyle(ButtonStyle.Secondary)
                    .WithCustomId($"{app.Id}-{EntityType.Application}");

                _ButtonsBuilder.Add(interactiveButton);
            }

            _AppName = CustomMessages.GetMessage(MessagesType.AppsFound);
            await this.CreateUserMessage();

            return _MessageWasSent;
        }

        public async Task<bool> GetAppAchivementsButtons(SteamApp steamApp)
        {
            ResetComponentValues();

            if (steamApp.Achievements != null && steamApp.Achievements.Count > 0)
            {
                foreach (SteamAppAchievement appAchievement in steamApp.Achievements.OrderBy(app => !app.Hidden))
                {
                    var interactiveButton = new ButtonBuilder()
                        .WithLabel(GetAppDisplayName(appAchievement.DisplayName, appAchievement.Hidden))
                        .WithStyle(ButtonStyle.Secondary)
                        .WithCustomId($"{steamApp.Id}_{appAchievement.Id}-{EntityType.Achivement}");

                    _ButtonsBuilder.Add(interactiveButton);
                }

                _AppName = CustomMessages.GetMessage(MessagesType.AchievementsFound, steamApp.Name);
                await this.CreateUserMessage();
            }

            return _MessageWasSent;
        }

        private static void ResetComponentValues()
        {
            _ButtonsBuilder = new List<ButtonBuilder>();
            _AppName = string.Empty;
            _Page = 0;
            _MessageWasSent = false;
        }

        private async Task CreateUserMessage()
        {
            _InstanceUserMessage = await _Context.Channel.SendMessageAsync(_AppName);

            await ((IUserMessage)_InstanceUserMessage).ModifyAsync(msg =>
            {
                msg.Components = GetButtonComponents().Build();
            });

            _Context.Client.InteractionCreated += HandleButtonEvent;
            _MessageWasSent = true;
        }

        private static string GetAppDisplayName(string displayName, bool isHidden = false)
        {
            if (!string.IsNullOrEmpty(displayName))
            {
                displayName += isHidden ? $" {CustomMessages.GetEmote(EmoteType.HiddenEye)}" : string.Empty;

                int inputLength = MeasureStringWidth(displayName);
                int spacesToAdd = _MaxLongForPixelWidth - inputLength;

                if (inputLength >= _MaxLongForPixelWidth || spacesToAdd <= 0)
                {
                    return displayName;
                }

                return displayName + new string('\u2800', (spacesToAdd / _BlankSpaceForPixelWidth));
            }

            return displayName;
        }

        private static ComponentBuilder GetButtonComponents()
        {
            var componentBuilder = new ComponentBuilder();

            for (int i = _MaxCountForPage * _Page; i < _ButtonsBuilder.Count && i < _MaxCountForPage * (_Page + 1); i++)
            {
                componentBuilder.WithButton(_ButtonsBuilder[i]);
            }

            if (_Page > 0)
            {
                componentBuilder.WithButton(_ArrowLeftMessage, _IdPreviousPage, ButtonStyle.Primary);
            }

            if (_MaxCountForPage * (_Page + 1) < _ButtonsBuilder.Count)
            {
                componentBuilder.WithButton(_ArrowRightMessage, _IdNextPage, ButtonStyle.Primary);
            }

            return componentBuilder;
        }

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
                    await GetResponseForButtonsEvent(buttonInteraction);
                    break;

            }

            await ((IUserMessage)_InstanceUserMessage).ModifyAsync(msg =>
            {
                msg.Content = _AppName;
                msg.Components = GetButtonComponents().Build();
            });
        }

        private static async Task GetResponseForButtonsEvent(SocketMessageComponent buttonInteraction)
        {
            if (buttonInteraction != null && buttonInteraction.Data.CustomId != null)
            {
                string customId = buttonInteraction.Data.CustomId;
                int index = customId.IndexOf("-");

                if (index != -1)
                {
                    try
                    {
                        string id = customId.Substring(0, index);
                        string entityType = customId.Substring(id.Length + 1, customId.Length - id.Length);


                        if (entityType == EntityType.Achivement.ToString())
                        {
                            _Apps

                            _OpenAiService.GetChatResponse();
                        }


                        await buttonInteraction.RespondAsync($"ID: {id} Type: {entityType}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    
                }
            }
        }

        private static int MeasureStringWidth(string appName)
        {
            Font fontSize = SystemFonts.DefaultFont;

            using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                SizeF size = g.MeasureString(appName, fontSize);
                return (int)size.Width;
            }
        }
    }
}
