using Utilities;

var bagCubesDictionary = new Dictionary<string, int>
{
    { "red", 12 },
    { "green", 13 },
    { "blue", 14 }
};

var sumOfPossibleGames = 0;
var sumOfPowerOfGames = 0;

try
{
    using var sr = Utility.ReadInputFileForProject(Directory.GetCurrentDirectory());

    while (await sr.ReadLineAsync() is { } line)
    {
        var gameAndData = line.Split(":");
        var game = gameAndData[0].Split(" ")[1];
        var gameNumber = int.Parse(game);

        var sets = gameAndData[1].Split(";");
        var isSetPossible = true;
        var minimumCubesDictionary = new Dictionary<string, int>();
        foreach (var set in sets)
        {
            var setDictionary = new Dictionary<string, int>();

            IEnumerable<string> cubesAndCount = set.Split(",").Select(x => x.Trim());
            foreach (var cubeAndCount in cubesAndCount)
            {
                var split = cubeAndCount.Split(" ");
                var count = int.Parse(split[0]);
                var color = split[1];
                setDictionary.Add(color, count);
            }

            var isGamePossible = setDictionary.Keys.All(color =>
                !bagCubesDictionary.ContainsKey(color) || bagCubesDictionary[color] >= setDictionary[color]);

            // Part 2
            // For each color add the minimum number of cubes needed to complete the set
            foreach (var color in setDictionary.Keys)
            {
                if (!minimumCubesDictionary.TryGetValue(color, out var value))
                    minimumCubesDictionary.Add(color, setDictionary[color]);
                else
                    minimumCubesDictionary[color] = Math.Max(value, setDictionary[color]);
            }

            if (isGamePossible) continue;

            isSetPossible = false;
        }

        if (isSetPossible) sumOfPossibleGames += gameNumber;

        // Part 2
        sumOfPowerOfGames += minimumCubesDictionary.Values.Aggregate(1, (sum, value) => sum * value);
    }

    Console.WriteLine($"Sum of possible games: {sumOfPossibleGames}");
    Console.WriteLine($"Sum of power of games: {sumOfPowerOfGames}");
}
catch (IOException e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
}