using Utilities;

var patterns = new List<List<List<char>>>();
var sumOfAllNotes = 0;

const int carriageReturn = 13;
const int eof = -1;

try
{
    using var sr = Utility.ReadInputFileForProject(Directory.GetCurrentDirectory());

    var currentPattern = new List<List<char>>();
    while (await sr.ReadLineAsync() is { } line)
    {
        if (!string.IsNullOrEmpty(line))
            currentPattern.Add(line.ToList());

        var isEndOfPattern = sr.Peek() == carriageReturn || sr.Peek() == eof;
        if (isEndOfPattern)
        {
            // Horizontal
            for (var i = 1; i < currentPattern.Count; i++)
            {
                var prevPattern = currentPattern[i - 1];
                var pattern = currentPattern[i];
                if (pattern.SequenceEqual(prevPattern))
                {
                    var isMirror = currentPattern.IsMirror(i);
                    if (isMirror)
                    {
                        // Console.WriteLine("Position: " + i);
                        Console.WriteLine("horizontal:" + patterns.Count);
                        sumOfAllNotes += i * 100;
                    }
                }
            }

            // Create vertical lines - could also use matrix transformation
            var verticalLines = new List<List<char>>();
            for (var i = 0; i < currentPattern[0].Count; i++)
            {
                var verticalLine = currentPattern.Select(symbols => symbols[i]).ToList();
                verticalLines.Add(verticalLine);
            }

            // Vertical
            for (var i = 1; i < verticalLines.Count; i++)
            {
                var prevPattern = verticalLines[i - 1];
                var pattern = verticalLines[i];
                if (pattern.SequenceEqual(prevPattern))
                {
                    var isMirror = verticalLines.IsMirror(i);
                    if (isMirror)
                    {
                        // Console.WriteLine("Position: " + i);
                        Console.WriteLine("vertical:" + patterns.Count);
                        sumOfAllNotes += i;
                    }
                }
            }

            patterns.Add(currentPattern);
            currentPattern.Clear();
        }
    }

    // 28558 - too high
    Console.WriteLine($"Sum of all notes: {sumOfAllNotes}");
}
catch (IOException e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
}

internal static class PatternExtension
{
    public static bool IsMirror(this List<List<char>> pattern, int startIndex)
    {
        var left = startIndex - 1;
        var right = startIndex;
        while (true)
        {
            var leftPrevPattern = pattern[left];
            var rightNextPattern = pattern[right];
            if (leftPrevPattern.SequenceEqual(rightNextPattern))
            {
                left--;
                right++;
            }
            else
                break;

            if (left < 0 || right > pattern.Count - 1)
                return true;
        }

        return false;
    }
}