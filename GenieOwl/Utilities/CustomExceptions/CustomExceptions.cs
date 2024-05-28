namespace GenieOwl.Utilities.CustomExceptions
{
    [Serializable]
    public class CustomExceptions : Exception
    {
        public CustomExceptions() { }

        public CustomExceptions(string message) : base(message) { }

        public CustomExceptions(string message, Exception inner) : base(message, inner) { }
    }
}
