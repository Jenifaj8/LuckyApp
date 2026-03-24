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
        public IActionResult Index(string numbers)
        {
            var inputNumbers = numbers.Split(',')
                .Select(n => int.Parse(n.Trim()))
                .ToList();

            List<int> result = new List<int>();

            for (int i = 0; i < inputNumbers.Count; i++)
            {
                int newNumber;

                if (i % 2 == 0)
                    newNumber = inputNumbers[i] + 1;
                else
                    newNumber = inputNumbers[i] - 1;

                if (newNumber < 0) newNumber = 0;
                if (newNumber > 9) newNumber = 9;

                result.Add(newNumber);
            }

            ViewBag.Result = string.Join("", result);

            return View();
        }
    }
}