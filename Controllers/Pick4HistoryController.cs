using LuckyApp.Data;
using LuckyApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;

namespace LuckyApp.Controllers
{
    public class Pick4HistoryController : Controller
    {
        private readonly LuckyAppDbContext _context;

        public Pick4HistoryController(
            LuckyAppDbContext context)
        {
            _context = context;
        }

        // Display historical results.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var draws = await _context.Pick4Draws
                .AsNoTracking()
                .OrderByDescending(d => d.DrawDate)
                .ThenBy(d => d.DrawType)
                .ToListAsync();

            return View(draws);
        }

        // Import historical results from a CSV file.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(
            IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] =
                    "Please select a CSV file.";

                return RedirectToAction(nameof(Index));
            }

            if (file.Length > 2 * 1024 * 1024 ||
                !Path.GetExtension(file.FileName)
                    .Equals(".csv",
                        StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] =
                    "Please select a CSV file under 2 MB.";

                return RedirectToAction(nameof(Index));
            }

            var existingDraws = await _context.Pick4Draws
                .AsNoTracking()
                .ToListAsync();

            var knownDraws = existingDraws.ToDictionary(
                d => (d.DrawDate, d.DrawType),
                d => d.WinningNumber);

            var newDraws = new List<Pick4Draw>();

            int skipped = 0;
            int invalid = 0;
            int conflicts = 0;

            try
            {
                using var stream = file.OpenReadStream();

                using var parser =
                    new TextFieldParser(stream);

                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;
                parser.TrimWhiteSpace = true;

                // Validate the CSV header.
                var header = parser.ReadFields();

                string[] expectedHeader =
                {
                    "DrawDate",
                    "DrawType",
                    "WinningNumber",
                    "Source"
                };

                if (header == null ||
                    !header.SequenceEqual(
                        expectedHeader,
                        StringComparer.OrdinalIgnoreCase))
                {
                    TempData["Error"] =
                        "The CSV header is incorrect.";

                    return RedirectToAction(nameof(Index));
                }

                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();

                    if (fields == null || fields.Length != 4)
                    {
                        invalid++;
                        continue;
                    }

                    if (!DateOnly.TryParseExact(
                            fields[0],
                            "yyyy-MM-dd",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out DateOnly drawDate))
                    {
                        invalid++;
                        continue;
                    }

                    string drawType = fields[1]
                        .Trim()
                        .ToLowerInvariant() switch
                    {
                        "midday" => "Midday",
                        "night" => "Night",
                        "evening" => "Night",
                        _ => string.Empty
                    };

                    string winningNumber = fields[2].Trim();

                    string source = fields[3].Trim();

                    if (string.IsNullOrEmpty(drawType) ||
                        winningNumber.Length != 4 ||
                        !winningNumber.All(
                            c => c >= '0' && c <= '9') ||
                        string.IsNullOrWhiteSpace(source) ||
                        source.Length > 500)
                    {
                        invalid++;
                        continue;
                    }

                    var key = (drawDate, drawType);

                    // Prevent duplicates and conflicting results.
                    if (knownDraws.TryGetValue(
                        key, out string? existingNumber))
                    {
                        if (existingNumber == winningNumber)
                        {
                            skipped++;
                        }
                        else
                        {
                            conflicts++;
                        }

                        continue;
                    }

                    newDraws.Add(new Pick4Draw
                    {
                        DrawDate = drawDate,
                        DrawType = drawType,
                        WinningNumber = winningNumber,
                        Source = source
                    });

                    knownDraws.Add(key, winningNumber);
                }

                // Save all new records.
                if (newDraws.Count > 0)
                {
                    _context.Pick4Draws.AddRange(newDraws);

                    await _context.SaveChangesAsync();
                }

                TempData["Success"] =
                    $"Import completed. " +
                    $"Added: {newDraws.Count}. " +
                    $"Duplicates skipped: {skipped}. " +
                    $"Invalid rows: {invalid}. " +
                    $"Conflicting results: {conflicts}.";
            }
            catch (MalformedLineException)
            {
                TempData["Error"] =
                    "The CSV file contains malformed data.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "The database import failed.";
            }
            catch (IOException)
            {
                TempData["Error"] =
                    "The CSV file could not be read.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}