using Microsoft.AspNetCore.Mvc;

namespace LuckyApp.Controllers
{
    public class KenoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string numbers, int pickCount, string drawType)
        {
            if (string.IsNullOrWhiteSpace(drawType))
            {
                ViewBag.Error = "Please select Midday or Evening.";
                ViewBag.SelectedPickCount = pickCount;
                return View();
            }

            if (string.IsNullOrWhiteSpace(numbers))
            {
                ViewBag.Error = "Please enter your Keno numbers.";
                ViewBag.SelectedPickCount = pickCount;
                return View();
            }

            var parts = numbers.Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(n => n.Trim())
                               .ToList();

            if (pickCount < 2 || pickCount > 10)
            {
                ViewBag.Error = "Pick count must be between 2 and 10.";
                ViewBag.SelectedPickCount = 2;
                return View();
            }

            if (parts.Count != pickCount)
            {
                ViewBag.Error = $"Keno requires exactly {pickCount} numbers for this selection.";
                ViewBag.SelectedPickCount = pickCount;
                return View();
            }

            List<int> inputNumbers = new();

            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int value))
                {
                    ViewBag.Error = "Please enter valid numbers only.";
                    ViewBag.SelectedPickCount = pickCount;
                    return View();
                }

                if (value < 1 || value > 70)
                {
                    ViewBag.Error = "Each Keno number must be between 1 and 70.";
                    ViewBag.SelectedPickCount = pickCount;
                    return View();
                }

                inputNumbers.Add(value);
            }

            if (inputNumbers.Distinct().Count() != pickCount)
            {
                ViewBag.Error = "Duplicate numbers are not allowed in Keno.";
                ViewBag.SelectedPickCount = pickCount;
                return View();
            }

            List<int> result = new();

            for (int i = 0; i < inputNumbers.Count; i++)
            {
                int newNumber = (i % 2 == 0) ? inputNumbers[i] + 2 : inputNumbers[i] - 1;

                if (newNumber < 1) newNumber = 1;
                if (newNumber > 70) newNumber = 70;

                result.Add(newNumber);
            }

            result = result.Distinct().OrderBy(n => n).ToList();

            ViewBag.SelectedDrawType = drawType;
            ViewBag.SelectedPickCount = pickCount;
            ViewBag.Result = string.Join(", ", result);

            return View();
        }
    }
}