namespace GenieOwl.Utilities.Messages
{
    public static class CustomMessages
    {
        private static Dictionary<MessagesType, string> _Messages = new()
        {
            { MessagesType.AppNotFound, "Game couldn't not be found. :rage: \n You can try search again! :innocent:" },
            { MessagesType.AppNotAchivement, "Game achivements couldn't not be found for app _{0}_. :rage:" },
            { MessagesType.AppsFound, "Could you be looking for any of these? :thinking_face:" },
            { MessagesType.AchievementsFound, "I found the following achievements for the game _{0}_" },
            { MessagesType.GenericError, "An error occurred, please report the bug on the GenieOwl forum. :slightly_smiling_face: \b _{0}_" },
            { MessagesType.HelpGameCommand, "You could use: !game <game name>" },
            { MessagesType.OwlReady, "GenieOwl is ready!" },
            { MessagesType.NextPage, "Next page" },
            { MessagesType.PreviousPage, "Previous page" },
            { MessagesType.PlaceHolderSelectMenu, "Select an achievement" },
            { MessagesType.UnhandledException, "Unhandled exception" },
        };

        private static Dictionary<EmoteType, string> _Emotes = new()
        {
            { EmoteType.LeftArrow, "⬅\ufe0f" },
            { EmoteType.RightArrow, "➡\ufe0f" },
            { EmoteType.HiddenEye, "\ud83d\udc41\u200d\ud83d\udde8" },
        };

        public static string GetEmote(EmoteType emoteType) => _Emotes[emoteType];

        public static string GetMessage(MessagesType messageType) => _Messages[messageType];

        public static string GetMessage(MessagesType messageType, string message) => String.Format(_Messages[messageType], message);

        public static string GetMessage(MessagesType messageType, List<string> message)
        {
            int parametersInMessage = _Messages[messageType].Count(msg => msg == '{');

            return (message.Count == parametersInMessage)
                ?
                String.Format(_Messages[messageType], message)
                :
                GetMessage(messageType);
        }

        public static Exception ResponseMessageEx(MessagesType messageType) => new Exception(_Messages[messageType]);
        
        public static Exception ResponseMessageEx(MessagesType messageType, Exception exception) => new Exception($"{_Messages[messageType]} \n Detail: {exception.Message}");
        
        public static Exception ResponseMessageEx(MessagesType messageType, string message) => new Exception(String.Format(_Messages[messageType], message));

        public static Exception ResponseMessageEx(MessagesType messageType, List<string> message)
        {
            int parametersInMessage = _Messages[messageType].Count(msg => msg == '{');

            return (message.Count == parametersInMessage)
                ?
                new Exception(String.Format(_Messages[messageType], message))
                :
                ResponseMessageEx(MessagesType.UnhandledException);
        }
    }
}
