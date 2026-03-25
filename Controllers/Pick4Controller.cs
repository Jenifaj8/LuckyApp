using Microsoft.AspNetCore.Mvc;
using LuckyApp.Models;
using LuckyApp.Services;

namespace LuckyApp.Controllers
{
    public class Pick4Controller : Controller
    {
        private readonly OlgPick4Service _olgPick4Service;

        public Pick4Controller(OlgPick4Service olgPick4Service)
        {
            _olgPick4Service = olgPick4Service;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await BuildInitialModelAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string inputNumber)
        {
            var model = await BuildInitialModelAsync();

            if (string.IsNullOrWhiteSpace(inputNumber))
            {
                model.ErrorMessage = "Please enter a 4-digit number.";
                return View(model);
            }

            inputNumber = inputNumber.Trim();

            if (inputNumber.Length != 4 || !inputNumber.All(char.IsDigit))
            {
                model.ErrorMessage = "Pick 4 requires exactly 4 digits.";
                return View(model);
            }

            model.InputNumber = inputNumber;
            model.GeneratedNumber = GenerateNextNumber(inputNumber);

            var gridResult = BuildFlipGrid(inputNumber);
            model.FlipGrid = gridResult.Grid;
            model.RowTotals = gridResult.RowTotals;

            return View(model);
        }

        private async Task<Pick4ViewModel> BuildInitialModelAsync()
        {
            var latest = await _olgPick4Service.GetLatestPick4ResultAsync();

            string latestDrawType = latest?.DrawType ?? "Midday";
            string latestDrawNumber = latest?.Number ?? "4566";
            string latestDrawDate = latest?.DrawDate ?? "Latest draw unavailable";

            string nextDrawType = latestDrawType.Equals("Midday", StringComparison.OrdinalIgnoreCase)
                ? "Evening"
                : "Midday";

            var gridResult = BuildFlipGrid(latestDrawNumber);

            return new Pick4ViewModel
            {
                LatestDrawDate = latestDrawDate,
                LatestDrawType = latestDrawType,
                LatestDrawNumber = latestDrawNumber,
                NextDrawType = nextDrawType,
                InputNumber = latestDrawNumber,
                GeneratedNumber = GenerateNextNumber(latestDrawNumber),
                FlipGrid = gridResult.Grid,
                RowTotals = gridResult.RowTotals
            };
        }

        private string GenerateNextNumber(string inputNumber)
        {
            var digits = inputNumber.Select(c => int.Parse(c.ToString())).ToList();
            List<int> generatedDigits = new();

            for (int i = 0; i < digits.Count; i++)
            {
                int newDigit = (i % 2 == 0) ? digits[i] + 1 : digits[i] - 1;

                if (newDigit < 0) newDigit = 0;
                if (newDigit > 9) newDigit = 9;

                generatedDigits.Add(newDigit);
            }

            return string.Join("", generatedDigits);
        }

        private (int[,] Grid, List<int> RowTotals) BuildFlipGrid(string inputNumber)
        {
            int[,] grid = new int[4, 4];

            // place input digits on the main diagonal
            for (int i = 0; i < 4; i++)
            {
                grid[i, i] = int.Parse(inputNumber[i].ToString());
            }

            int lastDigit = int.Parse(inputNumber[3].ToString());
            int nextDigit = (lastDigit + 1) % 10;

            // fill empty squares in order
            var fillOrder = new List<(int row, int col)>
            {
                (2, 3),
                (1, 3),
                (0, 3),

                (0, 2),
                (0, 1),

                (1, 0),
                (2, 0),
                (3, 0),

                (3, 1),
                (3, 2),

                (2, 1),
                (1, 2)
            };

            foreach (var cell in fillOrder)
            {
                grid[cell.row, cell.col] = nextDigit;
                nextDigit = (nextDigit + 1) % 10;
            }

            List<int> rowTotals = new();

            for (int row = 0; row < 4; row++)
            {
                int a = grid[row, 0];
                int b = grid[row, 1];
                int c = grid[row, 2];

                int firstPair = (a * 10) + b;
                int secondPair = (b * 10) + c;

                rowTotals.Add(firstPair + secondPair);
            }

            return (grid, rowTotals);
        }
    }
}