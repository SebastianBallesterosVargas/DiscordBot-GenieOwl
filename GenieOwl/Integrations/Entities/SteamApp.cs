namespace GenieOwl.Integrations.Entities
{
    public class SteamApp
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int PriceChangeValue { get; set; }
        public List<SteamAppAchievement>? Achievements { get; set; }
        public double? AppVersion { get; set; }
    }
}
