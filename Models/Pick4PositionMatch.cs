namespace LuckyApp.Models
{
    public class Pick4PositionMatch
    {
        public string PreviousNumber { get; set; } = string.Empty;

        public string WinningNumber { get; set; } = string.Empty;

        public string CalculatedNumber { get; set; } = string.Empty;

        public string Position { get; set; } = string.Empty;

        public List<int> RowTotals { get; set; } = new();

        public List<List<string>> MatchingTechniques
        {
            get;
            set;
        } = new();
    }
}