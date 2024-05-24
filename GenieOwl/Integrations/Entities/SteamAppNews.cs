namespace GenieOwl.Integrations.Entities
{
    public class SteamAppNews
    {
        public int AppId { get; set; }
        public string? Gid { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Contents { get; set; }
        public bool? IsExternalUrl { get; set; }
        public DateTime? Date { get; set; }
    }
}
