namespace GenieOwl.Common.Interfaces
{
    using Discord;
    using Discord.Commands;
    using GenieOwl.Utilities.Types;

    public interface IDiscordService
    {
        public Task CreateMessageResponse(SocketCommandContext context, List<ButtonBuilder> buttons, LenguageType lenguage);
    }
}
