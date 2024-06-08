namespace GenieOwl.Common.Services
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using GenieOwl.Common.Interfaces;
    using GenieOwl.Integrations.Entities;
    using GenieOwl.Integrations.Interfaces;
    using GenieOwl.Utilities.Messages;
    using GenieOwl.Utilities.Types;
    using Newtonsoft.Json;
    using SteamKit2;
    using System.Drawing;

    public class DiscordService : IDiscordService
    {
        private static IMessage _InstanceUserMessage;

        private static IPerplexityService _PerplexityService;

        private static SocketCommandContext _Context;

        private static List<ButtonBuilder> _ButtonsBuilder;

        private static LenguageType _Lenguage = LenguageType.English;

        private static string _AppName;

        private static int _Page = 0;

        private static readonly string _ArrowLeftMessage = $"{CustomEmotes.GetEmote(EmoteType.LeftArrow)} {CustomMessages.GetMessage(MessagesType.PreviousPage)}";

        private static readonly string _ArrowRightMessage = $"{CustomEmotes.GetEmote(EmoteType.RightArrow)} {CustomMessages.GetMessage(MessagesType.NextPage)}";

        private const string _IdPreviousPage = "previous_page";

        private const string _IdNextPage = "next_page";

        private const int _BlankSpaceForPixelWidth = 11;

        private const int _MaxLongForPixelWidth = 187;

        private const int _MaxCountForPage = 20;

        public DiscordService(IPerplexityService perplexityService, IDiscordIntegration discordIntegration)
        {
            _PerplexityService = perplexityService;
            discordIntegration.GetDiscordClient().InteractionCreated += HandleButtonEvent;
        }

        public async Task GetAppsButtons(List<SteamApp> steamApps, SocketCommandContext context, LenguageType lenguage)
        {
            InitializeComponentValues(context, lenguage);

            foreach (SteamApp app in steamApps)
            {
                var interactiveButton = new ButtonBuilder()
                    .WithLabel(GetAppDisplayName(app.Name))
                    .WithStyle(ButtonStyle.Secondary)
                    .WithCustomId(GetSerializedId(new DiscordButtonData
                    {
                        Id = app.Id,
                        Name = app.Name
                    }));

                _ButtonsBuilder.Add(interactiveButton);
            }

            _AppName = CustomMessages.GetMessage(MessagesType.AppsFound);
            await CreateUserMessage();
        }

        public async Task GetAppAchivementsButtons(SteamApp steamApp, SocketCommandContext context, LenguageType lenguage)
        {
            InitializeComponentValues(context, lenguage);

            if (steamApp.Achievements != null && steamApp.Achievements.Count > 0)
            {
                foreach (SteamAppAchievement appAchievement in steamApp.Achievements.OrderBy(app => !app.Hidden))
                {
                    var interactiveButton = new ButtonBuilder()
                        .WithLabel(GetAppDisplayName(appAchievement.DisplayName, appAchievement.Hidden))
                        .WithStyle(ButtonStyle.Secondary)
                        .WithCustomId(GetSerializedId(new DiscordButtonData
                        {
                            Id = steamApp.Id,
                            Name = steamApp.Name,
                            AchievementName = appAchievement.DisplayName
                        }));

                    _ButtonsBuilder.Add(interactiveButton);
                }

                _AppName = CustomMessages.GetMessage(MessagesType.AchievementsFound, steamApp.Name);
                await CreateUserMessage();
            }
        }

        private static void InitializeComponentValues(SocketCommandContext context, LenguageType lenguage)
        {
            _Lenguage = lenguage;
            _Context = context;
            _ButtonsBuilder = new List<ButtonBuilder>();
            _AppName = string.Empty;
            _Page = 0;
        }

        private static string GetSerializedId<T>(T entity)
        {
            return JsonConvert.SerializeObject(entity);
        }

        private static string GetAppDisplayName(string displayName, bool isHidden = false)
        {
            if (!string.IsNullOrEmpty(displayName))
            {
                char blankSpace = '\u2800';

                displayName += isHidden ? $" {CustomEmotes.GetEmote(EmoteType.HiddenEye)}" : string.Empty;

                int inputLength = MeasureStringWidth(displayName);
                int spacesToAdd = _MaxLongForPixelWidth - inputLength;

                if (inputLength >= _MaxLongForPixelWidth || spacesToAdd <= 0)
                {
                    return displayName;
                }

                return displayName + new string(blankSpace, (spacesToAdd / _BlankSpaceForPixelWidth));
            }

            return displayName;
        }

        private static async Task CreateUserMessage()
        {
            _InstanceUserMessage = await _Context.Channel.SendMessageAsync(_AppName);

            await ((IUserMessage)_InstanceUserMessage).ModifyAsync(msg =>
            {
                msg.Components = GetButtonComponents().Build();
            });
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
                    await HandleButtonOption(buttonInteraction);
                    break;
            }

            await ((IUserMessage)_InstanceUserMessage).ModifyAsync(msg =>
            {
                msg.Components = GetButtonComponents().Build();
            });

            await buttonInteraction.RespondAsync();
        }

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
                    await buttonInteraction.RespondAsync($"!game {discordButtonData?.Name}");
                }

                if (!string.IsNullOrEmpty(discordButtonData?.AchievementName))
                {
                    await buttonInteraction.RespondAsync(CustomMessages.GetMessage(MessagesType.GetMomment));

                    string guideMessage = await _PerplexityService.GetGuideForAchievement(discordButtonData, _Lenguage);

                    await buttonInteraction.Channel.SendMessageAsync(guideMessage);
                }
            }
        }

        private static int MeasureStringWidth(string appName)
        {
            Font fontSize = SystemFonts.DefaultFont;

            using (var bmp = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(bmp))
            {
                SizeF size = g.MeasureString(appName, fontSize);
                return (int)size.Width;
            }
        }
    }
}
