using Microsoft.AspNetCore.Mvc;

namespace LuckyApp.Controllers;

public class GeneratorController : Controller
{
    // GET
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
                newNumber = inputNumbers[i] + 2;
            else
                newNumber = inputNumbers[i] - 1;

            // keep numbers in range 1–49
            if (newNumber < 1) newNumber = 1;
            if (newNumber > 49) newNumber = 49;

            result.Add(newNumber);
        }

        // remove duplicates and sort
        result = result.Distinct().OrderBy(n => n).ToList();

        ViewBag.Result = string.Join(", ", result);

        return View();
    }
}