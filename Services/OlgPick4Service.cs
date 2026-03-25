using System.Text.RegularExpressions;
using LuckyApp.Models;

namespace LuckyApp.Services
{
    public class OlgPick4Service
    {
        private readonly HttpClient _httpClient;

        public OlgPick4Service(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Pick4LatestResult?> GetLatestPick4ResultAsync()
        {
            // Try the main results page first
            var urls = new[]
            {
                "https://www.olg.ca/en/lottery/winning-numbers-results.html",
                "https://www.olg.ca/en/lottery/play-pick-4/past-results.html"
            };

            foreach (var url in urls)
            {
                try
                {
                    var html = await _httpClient.GetStringAsync(url);
                    var result = ParsePick4Result(html);

                    if (result != null)
                        return result;
                }
                catch
                {
                    // ignore and try next URL
                }
            }

            return null;
        }

        private Pick4LatestResult? ParsePick4Result(string html)
        {
            // Looks for:
            // PICK-4. Winning Numbers. Sunday, March 22, 2026 - Midday Draw. 7. 3. 3. 7.
            var pattern = @"PICK-4\.\s*Winning Numbers\.\s*(?<date>[A-Za-z]+,\s+[A-Za-z]+\s+\d{1,2},\s+\d{4})\s*-\s*(?<draw>Midday|Evening)\s+Draw\.\s*(?<d1>\d)\.\s*(?<d2>\d)\.\s*(?<d3>\d)\.\s*(?<d4>\d)\.";
            var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (!match.Success)
                return null;

            return new Pick4LatestResult
            {
                DrawDate = match.Groups["date"].Value.Trim(),
                DrawType = match.Groups["draw"].Value.Trim(),
                Number = $"{match.Groups["d1"].Value}{match.Groups["d2"].Value}{match.Groups["d3"].Value}{match.Groups["d4"].Value}"
            };
        }
    }
}