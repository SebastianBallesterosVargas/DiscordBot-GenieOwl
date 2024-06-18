namespace GenieOwl.Utilities.Messages
{
    using Discord;
    using GenieOwl.Utilities.Types;

    public static class CustomMessages
    {
        private static readonly Dictionary<MessagesType, string> _Messages = new()
        {
            { MessagesType.AppNotFound, "Game couldn't not be found. :rage: \n You can try search again! :innocent:" },
            { MessagesType.AppNotAchivement, "Game achivements couldn't not be found for app {0}. :rage:" },
            { MessagesType.AppsFound, "Could you be looking for any of these? :thinking_face:" },
            { MessagesType.AchievementsFound, "I found the following achievements for the game" },
            { MessagesType.GenericError, "An error occurred, please report the bug on the GenieOwl forum. :slightly_smiling_face: \b _{0}_" },
            { MessagesType.GetMomment,  "Give me a momment... :face_with_monocle:" },
            { MessagesType.HelpGameCommand, "You could use: !game <game name>" },
            { MessagesType.OwlReady, "GenieOwl is ready!" },
            { MessagesType.PerplexityError, "An error occurred with the OpenAi service." },
            { MessagesType.NextPage, "Next page" },
            { MessagesType.NotSteamApps, "Not found Steam games in database" },
            { MessagesType.PreviousPage, "Previous page" },
            { MessagesType.PlaceHolderSelectMenu, "Select an achievement" },
            { MessagesType.UnhandledException, "Unhandled exception" },
        };

        public static string GetMessage(MessagesType messageType) => _Messages[messageType];

        public static string GetMessage(MessagesType messageType, string message) => String.Format(_Messages[messageType], message);

        public static string GetMessage(MessagesType messageType, string[] messages) => String.Format(_Messages[messageType], messages);

        public static Exception ResponseMessageEx(MessagesType messageType) => new Exception(_Messages[messageType]);
        
        public static Exception ResponseMessageEx(MessagesType messageType, Exception exception) => new Exception($"{_Messages[messageType]} \n Detail: {exception.Message}");
        
        public static Exception ResponseMessageEx(MessagesType messageType, string message) => new Exception(String.Format(_Messages[messageType], message));

        public static Exception ResponseMessageEx(MessagesType messageType, string[] messages) => new Exception(String.Format(_Messages[messageType], messages));
    }
}
