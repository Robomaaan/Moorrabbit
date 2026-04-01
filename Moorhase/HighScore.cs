namespace Moorhase
{
    public class HighScore
    {
        public string PlayerName { get; set; } = string.Empty;
        public int Points { get; set; }
        public string Mode { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
