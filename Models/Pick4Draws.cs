using System.ComponentModel.DataAnnotations;

namespace LuckyApp.Models
{
    public class Pick4Draw
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateOnly DrawDate { get; set; }

        [Required]
        [MaxLength(10)]
        public string DrawType { get; set; } = string.Empty;

        [Required]
        [StringLength(4, MinimumLength = 4)]
        [RegularExpression(@"^[0-9]{4}$")]
        public string WinningNumber { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Source { get; set; }
    }
}
