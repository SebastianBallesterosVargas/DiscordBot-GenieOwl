namespace GenieOwl.Integrations.Entities
{
    public class OpenAiChoices
    {
        public string? FinishReason { get; set; }

        public int? Index { get; set; }

        public OpenAiMessage? Message { get; set; }
    }
}
