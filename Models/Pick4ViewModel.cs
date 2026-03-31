namespace LuckyApp.Models
{
    public class Pick4ViewModel
    {
        public string? LatestDrawDate { get; set; }
        public string? LatestDrawType { get; set; }
        public string? LatestDrawNumber { get; set; }
        public string? NextDrawType { get; set; }
        public string? InputNumber { get; set; }
        public string? GeneratedNumber { get; set; }
        public string? ErrorMessage { get; set; }

        public int[,] FlipGrid { get; set; } = new int[4, 4];
        public List<int> RowTotals { get; set; } = new List<int>();
    }
}