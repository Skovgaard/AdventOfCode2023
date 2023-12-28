using System.Collections.Immutable;
using Utilities;

// --- Part One ---

var sumOfCalibrationValues = 0;

try
{
    using var sr = Utility.ReadInputFileForProject(Directory.GetCurrentDirectory());

    while (await sr.ReadLineAsync() is { } line)
    {
        var firstNumber = FindFirstNumber(line);
        var lastNumber = LastFirstNumber(line);
        var numbersString = firstNumber.ToString() + lastNumber.ToString();
        var parsedNumber = int.Parse(numbersString);
        sumOfCalibrationValues += parsedNumber;
    }

    Console.WriteLine($"PART 1: Sum of calibration values: {sumOfCalibrationValues}");
}
catch (IOException e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
}

// --- Part Two ---

var letterNumbers = new Dictionary<string, int>
{
    { "one", 1 },
    { "two", 2 },
    { "three", 3 },
    { "four", 4 },
    { "five", 5 },
    { "six", 6 },
    { "seven", 7 },
    { "eight", 8 },
    { "nine", 9 }
};

IEnumerable<string> substringValues = letterNumbers.Keys.ToArray()
    .Concat(letterNumbers.Values.Select(x => x.ToString()).ToArray()).ToImmutableArray();

var sumOfCalibrationValuesPartTwo = 0;

try
{
    using var sr = Utility.ReadInputFileForProject(Directory.GetCurrentDirectory());

    while (await sr.ReadLineAsync() is { } line)
    {
        var (firstNumber, lastNumber) = FindFirstAndLastSubstringsInString(line, substringValues);
        var numbersString = firstNumber + lastNumber;
        var parsedNumber = int.Parse(numbersString);
        sumOfCalibrationValuesPartTwo += parsedNumber;
    }

    Console.WriteLine($"PART 2: Sum of calibration values: {sumOfCalibrationValuesPartTwo}");
}
catch (IOException e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
}

return;

char LastFirstNumber(string input) => input.Where(char.IsDigit).LastOrDefault();
char FindFirstNumber(string input) => input.Where(char.IsDigit).FirstOrDefault();

(string First, string Last) FindFirstAndLastSubstringsInString(string input, IEnumerable<string> substrings)
{
    int? firstIndex = null;
    int? lastIndex = null;
    var firstSubstring = string.Empty;
    var lastSubstring = string.Empty;

    foreach (var substring in substrings)
    {
        if (!input.Contains(substring, StringComparison.Ordinal)) continue;

        var indexOf = input.IndexOf(substring, StringComparison.Ordinal);
        if (firstIndex is null || indexOf < firstIndex)
        {
            firstIndex = indexOf;
            firstSubstring = substring;
        }

        var lastIndexOf = input.LastIndexOf(substring, StringComparison.Ordinal);
        if (lastIndex is null || lastIndexOf > lastIndex)
        {
            lastIndex = lastIndexOf;
            lastSubstring = substring;
        }
    }

    if (string.IsNullOrEmpty(firstSubstring) || string.IsNullOrEmpty(lastSubstring))
        throw new NotSupportedException("Could not find substring match in string");

    if (letterNumbers.TryGetValue(firstSubstring, out var firstValue))
        firstSubstring = firstValue.ToString();

    if (letterNumbers.TryGetValue(lastSubstring, out var lastValue))
        lastSubstring = lastValue.ToString();

    return (firstSubstring, lastSubstring);
}
