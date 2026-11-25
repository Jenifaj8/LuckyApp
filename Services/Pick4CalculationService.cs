using LuckyApp.Models;

namespace LuckyApp.Services
{
    public class Pick4CalculationService
    {
        public List<Pick4ArithmeticResult> CalculateArithmetic(
            List<int> totals)
        {
            var results = new List<Pick4ArithmeticResult>();

            AddTechnique(results, totals,
                "First Digit", d => d[0]);

            AddTechnique(results, totals,
                "Last Digit", d => d[^1]);

            AddTechnique(results, totals,
                "Middle Digit",
                d => d.Length == 3 ? d[1] : null);

            AddTechnique(results, totals,
                "Add All Digits", d => d.Sum());

            AddTechnique(results, totals,
                "Last Digit - Next Up",
                d => (d[^1] + 1) % 10);

            AddTechnique(results, totals,
                "Last Digit - Next Down",
                d => (d[^1] + 9) % 10);

            AddTechnique(results, totals,
                "First Digit - Next Up",
                d => (d[0] + 1) % 10);

            AddTechnique(results, totals,
                "First + Second Digit",
                d => d.Length >= 2
                    ? d[0] + d[1] : null);

            AddTechnique(results, totals,
                "First + Last Digit",
                d => d[0] + d[^1]);

            AddTechnique(results, totals,
                "First - Last Digit",
                d => d[0] - d[^1]);

            AddTechnique(results, totals,
                "Last - First Digit",
                d => d[^1] - d[0]);

            AddTechnique(results, totals,
                "Second - First Digit",
                d => d.Length >= 2
                    ? d[1] - d[0] : null);

            AddTechnique(results, totals,
                "First Digit - Digit Count",
                d => d[0] - d.Length);

            AddTechnique(results, totals,
                "Sum + First Digit",
                d => d.Sum() + d[0]);

            AddTechnique(results, totals,
                "Absolute Difference",
                d => Math.Abs(d[0] - d[^1]));

            AddTechnique(results, totals,
                "Sum + Digit Count Difference",
                d =>
                {
                    int total = d.Sum() + d.Length;

                    int[] digits = total.ToString()
                        .Select(c => c - '0')
                        .ToArray();

                    return digits[^1] - digits[0];
                });

            AddTechnique(results, totals,
                "Mirror Last Digit",
                d => (d[^1] + 5) % 10);

            return results;
        }

        private void AddTechnique(
            List<Pick4ArithmeticResult> results,
            List<int> totals,
            string technique,
            Func<int[], int?> calculation)
        {
            var result = new Pick4ArithmeticResult
            {
                Technique = technique
            };

            foreach (int total in totals)
            {
                int[] digits = total.ToString()
                    .Select(c => c - '0')
                    .ToArray();

                int? value = calculation(digits);

                // Only accept single digits from 0 to 9.
                if (value.HasValue &&
                    value.Value >= 0 &&
                    value.Value <= 9)
                {
                    result.Values.Add(value);
                }
                else
                {
                    result.Values.Add(null);
                }
            }

            results.Add(result);
        }
    }
}