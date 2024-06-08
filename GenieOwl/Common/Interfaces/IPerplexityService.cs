namespace GenieOwl.Common.Interfaces
{
    using GenieOwl.Integrations.Entities;
    using GenieOwl.Utilities.Types;

    public interface IPerplexityService
    {
        public Task<string> GetGuideForAchievement(DiscordButtonData discordButtonData, LenguageType lenguage);
    }
}
