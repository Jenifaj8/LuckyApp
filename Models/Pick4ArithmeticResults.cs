namespace LuckyApp.Models
{
    public class Pick4ArithmeticResult
    {
        public string Technique { get; set; } = string.Empty;

        public List<int?> Values { get; set; } = new();
    }
}