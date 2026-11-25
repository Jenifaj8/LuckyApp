using LuckyApp.Models;

namespace LuckyApp.Services
{
    public class Pick4PositionService
    {
        // All 24 possible position arrangements.
        public List<string> GetPositions()
        {
            return new List<string>
            {
                "1234", "1243", "1324", "1342",
                "1423", "1432", "2134", "2143",
                "2314", "2341", "2413", "2431",
                "3124", "3142", "3214", "3241",
                "3412", "3421", "4123", "4132",
                "4213", "4231", "4312", "4321"
            };
        }

        // Rearrange the four calculated digits.
        public string ApplyPosition(
            string number,
            string position)
        {
            if (number == null ||
                number.Length != 4 ||
                !number.All(c => c >= '0' && c <= '9'))
            {
                throw new ArgumentException(
                    "Number must contain exactly four digits.");
            }

            if (!GetPositions().Contains(position))
            {
                throw new ArgumentException(
                    "Invalid position arrangement.");
            }

            char[] arrangedNumber = new char[4];

            for (int i = 0; i < 4; i++)
            {
                int destination = position[i] - '1';

                arrangedNumber[destination] = number[i];
            }

            return new string(arrangedNumber);
        }

        // Reconstruct a known historical winning number.
        public List<Pick4PositionMatch> FindHistoricalMatches(
            string previousNumber,
            string winningNumber,
            List<int> rowTotals,
            List<Pick4ArithmeticResult> arithmeticResults)
        {
            var matches = new List<Pick4PositionMatch>();

            if (previousNumber == null ||
                previousNumber.Length != 4 ||
                !previousNumber.All(c => c >= '0' && c <= '9') ||
                winningNumber == null ||
                winningNumber.Length != 4 ||
                !winningNumber.All(c => c >= '0' && c <= '9') ||
                rowTotals.Count != 4)
            {
                return matches;
            }

            foreach (string position in GetPositions())
            {
                var matchingTechniques =
                    new List<List<string>>();

                char[] calculatedDigits = new char[4];

                bool validMatch = true;

                for (int row = 0; row < 4; row++)
                {
                    int destination = position[row] - '1';

                    int requiredDigit =
                        winningNumber[destination] - '0';

                    var techniques = arithmeticResults
                        .Where(result =>
                            result.Values.Count == 4 &&
                            result.Values[row] == requiredDigit)
                        .Select(result => result.Technique)
                        .Distinct()
                        .ToList();

                    if (techniques.Count == 0)
                    {
                        validMatch = false;
                        break;
                    }

                    matchingTechniques.Add(techniques);

                    calculatedDigits[row] =
                        (char)('0' + requiredDigit);
                }

                if (validMatch)
                {
                    matches.Add(new Pick4PositionMatch
                    {
                        PreviousNumber = previousNumber,

                        WinningNumber = winningNumber,

                        CalculatedNumber =
                            new string(calculatedDigits),

                        Position = position,

                        RowTotals = new List<int>(rowTotals),

                        MatchingTechniques = matchingTechniques
                    });
                }
            }

            return matches;
        }
    }
}