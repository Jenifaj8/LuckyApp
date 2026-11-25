using Microsoft.AspNetCore.Mvc;
using LuckyApp.Models;
using LuckyApp.Services;

namespace LuckyApp.Controllers
{
    public class Pick4Controller : Controller
    {
        private readonly OlgPick4Service _olgPick4Service;
        private readonly Pick4CalculationService _pick4CalculationService;

        public Pick4Controller(
            OlgPick4Service olgPick4Service,
            Pick4CalculationService pick4CalculationService)
        {
            _olgPick4Service = olgPick4Service;
            _pick4CalculationService = pick4CalculationService;
        }

        // Load the Pick 4 page.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await BuildInitialModelAsync();

            return View(model);
        }

        // Process the number entered by the user.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string? inputNumber)
        {
            var model = await BuildInitialModelAsync();

            inputNumber = inputNumber?.Trim();

            model.InputNumber = inputNumber;

            // Validate the entered number.
            if (!IsValidNumber(inputNumber))
            {
                model.ErrorMessage =
                    "Please enter exactly 4 digits.";

                model.RowTotals.Clear();
                model.ArithmeticResults.Clear();

                return View(model);
            }

            // Generate the flip grid and arithmetic calculations.
            ApplyCalculations(model, inputNumber!);

            return View(model);
        }

        // Retrieve the latest OLG result.
        private async Task<Pick4ViewModel> BuildInitialModelAsync()
        {
            var latest = await _olgPick4Service
                .GetLatestPick4ResultAsync();

            var model = new Pick4ViewModel
            {
                LatestDrawDate = latest?.DrawDate,

                LatestDrawType = latest?.DrawType,

                LatestDrawNumber = latest?.Number,

                NextDrawType = latest == null
                    ? "Unavailable"
                    : string.Equals(
                        latest.DrawType,
                        "Midday",
                        StringComparison.OrdinalIgnoreCase)
                        ? "Night"
                        : "Midday"
            };

            // Automatically calculate if a valid result exists.
            if (IsValidNumber(latest?.Number))
            {
                model.InputNumber = latest!.Number;

                ApplyCalculations(model, latest.Number);
            }

            return model;
        }

        // Apply the flip grid and arithmetic calculations.
        private void ApplyCalculations(
            Pick4ViewModel model,
            string inputNumber)
        {
            var (grid, totals) = BuildFlipGrid(inputNumber);

            model.FlipGrid = grid;

            model.RowTotals = totals;

            model.ArithmeticResults =
                _pick4CalculationService.CalculateArithmetic(totals);
        }

        // Validate that the input contains exactly four digits.
        private bool IsValidNumber(string? number)
        {
            return number != null
                   && number.Length == 4
                   && number.All(c => c >= '0' && c <= '9');
        }

        // Build the handwritten flip technique grid.
        private (int[,] grid, List<int> totals)
            BuildFlipGrid(string inputNumber)
        {
            int[,] grid = new int[4, 4];

            // Place the winning number on the diagonal.
            for (int i = 0; i < 4; i++)
            {
                grid[i, i] = inputNumber[i] - '0';
            }

            // Start counting from the last digit.
            int lastDigit = inputNumber[3] - '0';

            int nextDigit = (lastDigit + 1) % 10;

            // Fill the remaining cells in the correct sequence.
            var fillOrder = new List<(int row, int col)>
            {
                (3, 2),
                (3, 1),
                (3, 0),

                (2, 0),
                (1, 0),

                (0, 1),
                (0, 2),
                (0, 3),

                (1, 3),
                (2, 3),

                (2, 1),
                (1, 2)
            };

            foreach (var (row, col) in fillOrder)
            {
                grid[row, col] = nextDigit;

                nextDigit = (nextDigit + 1) % 10;
            }

            // Calculate the total for each row.
            var totals = new List<int>();

            for (int row = 0; row < 4; row++)
            {
                int firstPair =
                    grid[row, 0] * 10 + grid[row, 1];

                int secondPair =
                    grid[row, 2] * 10 + grid[row, 3];

                totals.Add(firstPair + secondPair);
            }

            return (grid, totals);
        }
    }
}