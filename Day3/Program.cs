using System.Text.RegularExpressions;
using Utilities;

const char emptyValue = '.';

try
{
    using var sr = Utility.ReadInputFileForProject(Directory.GetCurrentDirectory());

    var lines = new List<string>();
    var numberLines = new List<List<Match>>();
    var asteriskLines = new List<List<Match>>();

    var sum = 0;
    var partTwoSum = 0;

    while (await sr.ReadLineAsync() is { } line)
    {
        lines.Add(line);
        var matches = FindNumbersInLine(line);
        numberLines.Add(matches.ToList());
        var asterisks = FindAsteriskInLine(line);
        asteriskLines.Add(asterisks.ToList());
    }

    for (var i = 0; i < lines.Count; i++)
    {
        foreach (var match in numberLines[i])
        {
            var index = match.Index;
            var length = match.Length;

            // Above
            if (i != 0)
            {
                var startIndex = index - 1 > 0 ? index - 1 : index;
                var substringLength = index + length + 1 < lines[i].Length - 1 && index != 0 ? length + 2 : length + 1;
                var stringAbove = lines[i - 1].Substring(startIndex, substringLength);
                if (ContainsSymbol(stringAbove))
                {
                    sum += int.Parse(match.Value);
                    continue;
                }
            }

            // Before
            var indexBefore = index - 1;
            var charBefore = indexBefore > 0 && ContainsSymbol(lines[i][indexBefore].ToString());
            if (charBefore)
            {
                sum += int.Parse(match.Value);
                continue;
            }

            // After
            var indexAfter = index + length;
            var charAfter = indexAfter < lines[i].Length - 1 && ContainsSymbol(lines[i][indexAfter].ToString());
            if (charAfter)
            {
                sum += int.Parse(match.Value);
                continue;
            }

            // Below
            if (i != lines.Count - 1)
            {
                var startIndex = index - 1 > 0 ? index - 1 : index;
                var substringLength = index + length + 1 < lines[i].Length - 1 || index == 0 ? length + 2 : length + 1;
                var stringBelow = lines[i + 1].Substring(startIndex, substringLength);
                if (ContainsSymbol(stringBelow))
                    sum += int.Parse(match.Value);
            }
        }
    }

    // Part 2
    for (var i = 0; i < lines.Count; i++)
    {
        foreach (var asteriskLine in asteriskLines[i])
        {
            var adjacentNumbers = new HashSet<Match>();
            var x = asteriskLine.Index > 0 ? asteriskLine.Index - 1 : 0;
            var y = i - 1;

            // Above
            if (i >= 0)
            {
                if (char.IsNumber(lines[y][x])) FindAndAddNumber(numberLines, adjacentNumbers, x, y);
                x++;
                if (char.IsNumber(lines[y][x])) FindAndAddNumber(numberLines, adjacentNumbers, x, y);
                x++;
                if (char.IsNumber(lines[y][x])) FindAndAddNumber(numberLines, adjacentNumbers, x, y);
                x -= 2;
            }

            y++;

            // Beside
            if (char.IsNumber(lines[y][x])) FindAndAddNumber(numberLines, adjacentNumbers, x, y);
            x += 2;
            if (char.IsNumber(lines[y][x])) FindAndAddNumber(numberLines, adjacentNumbers, x, y);
            x -= 2;
            y++;

            // Below
            if (y < lines.Count)
            {
                if (char.IsNumber(lines[y][x])) FindAndAddNumber(numberLines, adjacentNumbers, x, y);
                x++;
                if (char.IsNumber(lines[y][x])) FindAndAddNumber(numberLines, adjacentNumbers, x, y);
                x++;
                if (char.IsNumber(lines[y][x])) FindAndAddNumber(numberLines, adjacentNumbers, x, y);
            }

            if (adjacentNumbers.Count >= 2)
                partTwoSum += adjacentNumbers.Aggregate(1, (acc, next) => acc * int.Parse(next.Value));
        }
    }

    Console.WriteLine($"PART 1: The sum of all numbers that are surrounded by symbols is {sum}");
    Console.WriteLine($"PART 2: The sum of all numbers that surrounds an asterisk is {partTwoSum}");
}
catch (IOException e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
}

return;

IEnumerable<Match> FindNumbersInLine(string line)
{
    var regex = NumberRegex();
    var matches = regex.Matches(line);
    return matches.ToArray();
}

IEnumerable<Match> FindAsteriskInLine(string line)
{
    var regex = AsteriskRegex();
    var matches = regex.Matches(line);
    return matches.ToArray();
}

bool ContainsSymbol(string substring) =>
    substring.Any(x => x != emptyValue && !char.IsNumber(x));

// Part 2
void FindAndAddNumber(IReadOnlyList<List<Match>> numberLines, ISet<Match> adjacentNumbers, int x, int y)
{
    var match = numberLines[y].FirstOrDefault(match => x >= match.Index && x <= match.Index + match.Length);
    if (match is not null)
        adjacentNumbers.Add(match);
}

internal partial class Program
{
    [GeneratedRegex(@"\d+")]
    private static partial Regex NumberRegex();

    [GeneratedRegex(@"\*")]
    private static partial Regex AsteriskRegex();
}