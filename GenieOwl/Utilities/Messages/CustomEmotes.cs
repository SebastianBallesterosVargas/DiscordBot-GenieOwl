namespace GenieOwl.Utilities.Messages
{
    using GenieOwl.Utilities.Types;

    public static class CustomEmotes
    {
        private static readonly Dictionary<EmoteType, string> _Emotes = new()
        {
            { EmoteType.LeftArrow, "⬅\ufe0f" },
            { EmoteType.RightArrow, "➡\ufe0f" },
            { EmoteType.HiddenEye, "\ud83d\udc41\u200d\ud83d\udde8" }
        };

        public static string GetEmote(EmoteType emoteType) => _Emotes[emoteType];
    }
}
