namespace GenieOwl.Integrations.Entities
{
    public class OpenAiMessageResponse
    {
        public string? Id { get; set; }
        public string? RequestId { get; set; }
        public List<OpenAiChoices>? Choices { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
