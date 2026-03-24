using Microsoft.AspNetCore.Mvc;

namespace LuckyApp.Controllers
{
    public class Pick4Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string pick4Number, string drawType)
        {
            if (string.IsNullOrWhiteSpace(drawType))
            {
                ViewBag.Error = "Please select Midday or Evening.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(pick4Number))
            {
                ViewBag.Error = "Please enter exactly 4 digits.";
                return View();
            }

            pick4Number = pick4Number.Trim();

            if (pick4Number.Length != 4 || !pick4Number.All(char.IsDigit))
            {
                ViewBag.Error = "Pick 4 requires exactly 4 digits.";
                return View();
            }

            var digits = pick4Number.Select(c => int.Parse(c.ToString())).ToList();
            List<int> result = new();

            for (int i = 0; i < digits.Count; i++)
            {
                int newDigit = (i % 2 == 0) ? digits[i] + 1 : digits[i] - 1;

                if (newDigit < 0) newDigit = 0;
                if (newDigit > 9) newDigit = 9;

                result.Add(newDigit);
            }

            ViewBag.SelectedDrawType = drawType;
            ViewBag.Result = string.Join("", result);

            return View();
        }
    }
}