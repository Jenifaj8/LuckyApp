using Microsoft.AspNetCore.Mvc;

namespace LuckyApp.Controllers
{
    public class DailyGrandController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string mainNumbers, int? grandNumber)
        {
            if (string.IsNullOrWhiteSpace(mainNumbers) || grandNumber == null)
            {
                ViewBag.Error = "Please enter 5 main numbers and 1 grand number.";
                return View();
            }

            var parts = mainNumbers.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(n => n.Trim())
                                   .ToList();

            if (parts.Count != 5)
            {
                ViewBag.Error = "Daily Grand requires exactly 5 main numbers.";
                return View();
            }

            List<int> inputNumbers = new();

            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int value))
                {
                    ViewBag.Error = "Please enter valid main numbers only.";
                    return View();
                }

                if (value < 1 || value > 49)
                {
                    ViewBag.Error = "Each main number must be between 1 and 49.";
                    return View();
                }

                inputNumbers.Add(value);
            }

            if (inputNumbers.Distinct().Count() != 5)
            {
                ViewBag.Error = "Duplicate main numbers are not allowed.";
                return View();
            }

            if (grandNumber < 1 || grandNumber > 7)
            {
                ViewBag.Error = "Grand Number must be between 1 and 7.";
                return View();
            }

            List<int> result = new();

            for (int i = 0; i < inputNumbers.Count; i++)
            {
                int newNumber = (i % 2 == 0) ? inputNumbers[i] + 2 : inputNumbers[i] - 1;

                if (newNumber < 1) newNumber = 1;
                if (newNumber > 49) newNumber = 49;

                result.Add(newNumber);
            }

            int newGrandNumber = grandNumber.Value + 1;
            if (newGrandNumber > 7) newGrandNumber = 1;

            result = result.Distinct().OrderBy(n => n).ToList();

            ViewBag.Result = string.Join(", ", result);
            ViewBag.GrandResult = newGrandNumber;

            return View();
        }
    }
}