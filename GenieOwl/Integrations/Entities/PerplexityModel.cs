namespace GenieOwl.Integrations.Entities
{
    public class PerplexityModel
    {
        public string model { get; set; }

        public List<PerplexityMessage> messages { get; set; }
    }
}
