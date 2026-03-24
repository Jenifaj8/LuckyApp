using Microsoft.AspNetCore.Mvc;

namespace LuckyApp.Controllers
{
    public class Lotto649Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string numbers)
        {
            if (string.IsNullOrWhiteSpace(numbers))
            {
                ViewBag.Error = "Please enter exactly 6 numbers.";
                return View();
            }

            var parts = numbers.Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(n => n.Trim())
                               .ToList();

            if (parts.Count != 6)
            {
                ViewBag.Error = "Lotto 6/49 requires exactly 6 numbers.";
                return View();
            }

            List<int> inputNumbers = new();

            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int value))
                {
                    ViewBag.Error = "Please enter valid numbers only.";
                    return View();
                }

                if (value < 1 || value > 49)
                {
                    ViewBag.Error = "Each number must be between 1 and 49.";
                    return View();
                }

                inputNumbers.Add(value);
            }

            if (inputNumbers.Distinct().Count() != 6)
            {
                ViewBag.Error = "Duplicate numbers are not allowed for Lotto 6/49.";
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

            result = result.Distinct().OrderBy(n => n).ToList();

            ViewBag.Result = string.Join(", ", result);
            return View();
        }
    }
}