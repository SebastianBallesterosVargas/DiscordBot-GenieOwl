namespace GenieOwl.Integrations.Entities
{
    public class OpenAiMessage
    {
        public string? Content { get; set; }
        public string? TextContent { get; set; }
        public string? Role { get; set; }
        public List<string>? Images { get; set; }
    }
}
