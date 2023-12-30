using Utilities;

var sumOfPoints = 0;
var scratchCards = new List<int>();

try
{
    using var sr = Utility.ReadInputFileForProject(Directory.GetCurrentDirectory());

    var lineNumber = 0;

    while (await sr.ReadLineAsync() is { } line)
    {
        var numbers = line.Split(":")[1].Split("|");
        var winningNumbers = numbers.First().Split(" ").Where(x => !string.IsNullOrEmpty(x)).ToArray();
        var numbersYouHave = numbers.Last().Split(" ").Where(x => !string.IsNullOrEmpty(x)).ToArray();

        var points = 0;
        var timesWon = 0;

        foreach (var winningNumber in winningNumbers)
        {
            if (numbersYouHave.Contains(winningNumber))
            {
                if (points == 0)
                {
                    points = 1;
                    timesWon++;
                }
                else
                {
                    points *= 2;
                    timesWon++;
                }
            }
        }

        sumOfPoints += points;

        // Part 2
        for (var i = 0; i < timesWon + 1; i++)
        {
            var toAdd = 1;
            if (i != 0) toAdd += scratchCards[lineNumber] - 1;
            AddOrIncrementScratchCard(lineNumber + i, toAdd);
        }

        lineNumber++;
    }

    Console.WriteLine($"PART 1: Sum of points: {sumOfPoints}");
    Console.WriteLine($"PART 2: Number of scratch cards: {scratchCards.Sum()}");
}
catch (IOException e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
}

return;

void AddOrIncrementScratchCard(int index, int toAdd)
{
    try
    {
        scratchCards[index] += toAdd;
    }
    catch (ArgumentOutOfRangeException)
    {
        scratchCards.Add(toAdd);
    }
}