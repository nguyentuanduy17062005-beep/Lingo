namespace LingoAppNet8.Models
{
    public class TenseData
    {
        public int TenseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string VietnameseName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Structure { get; set; } = string.Empty;
        public string Usage { get; set; } = string.Empty;
        public string Examples { get; set; } = string.Empty;
        public string TimeMarkers { get; set; } = string.Empty;
        public int Level { get; set; }
    }
}
