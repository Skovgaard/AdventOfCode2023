using Utilities;

string? navigation = null;
const string startNode = "AAA";
const string destinationNode = "ZZZ";

var nodes = new Dictionary<string, (string, string)>();

try
{
    using var sr = Utility.ReadInputFileForProject(Directory.GetCurrentDirectory());

    var lineNumber = 1;

    while (await sr.ReadLineAsync() is { } line)
    {
        switch (lineNumber)
        {
            case 1:
                navigation = line;
                break;
            case >= 3:
            {
                var split = line.Split('=');
                var key = split[0].Trim();
                var value = split[1].Replace("(", "").Replace(")", "").Trim();
                var valueSplit = value.Split(',');
                var left = valueSplit[0].Trim();
                var right = valueSplit[1].Trim();

                nodes.Add(key, (left, right));
                break;
            }
        }

        lineNumber++;
    }

    if (navigation is null) throw new Exception("Navigation is null");

    var steps = 0;
    var currentNode = startNode;

    while (currentNode != destinationNode)
    {
        var direction = navigation[steps % navigation.Length];
        var (left, right) = nodes[currentNode];
        currentNode = direction switch
        {
            'L' => left,
            'R' => right,
            _ => throw new Exception("Invalid direction")
        };

        steps++;
    }

    Console.WriteLine($"Part 1: Steps: {steps}");

    // Part 2
    var stepsPart2 = 0;
    var currentNodes = nodes.Keys.Where(x => x.EndsWith('A')).ToArray();
    var stepsToFirstDestination = new int[currentNodes.Length];
    var stepsToSecondDestination = new int[currentNodes.Length];

    while (true)
    {
        stepsPart2++;

        var direction = navigation[stepsPart2 % navigation.Length];
        for (var i = 0; i < currentNodes.Length; i++)
        {
            var (left, right) = nodes[currentNodes[i]];
            currentNodes[i] = direction switch
            {
                'L' => left,
                'R' => right,
                _ => throw new Exception("Invalid direction")
            };

            if (currentNodes[i].EndsWith('Z') && stepsToFirstDestination[i] != 0 && stepsToSecondDestination[i] == 0)
                stepsToSecondDestination[i] = stepsPart2;
            if (currentNodes[i].EndsWith('Z') && stepsToFirstDestination[i] == 0)
                stepsToFirstDestination[i] = stepsPart2;
        }

        // Find the least common multiple of all the steps to the first destination
        // https://en.wikipedia.org/wiki/Least_common_multiple
        if (stepsToSecondDestination.All(x => x != 0))
        {
            var dif = stepsToSecondDestination.Select((x, idx) => (long)x - stepsToFirstDestination[idx]).ToArray();
            var lcm = LcmOfArray(dif);
            Console.WriteLine($"Part 2: Steps: {lcm}");
            break;
        }
    }
}
catch (IOException e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
}

return;

static long LcmOfArray(long[] numbers)
{
    var lcm = numbers[0];

    for (var i = 1; i < numbers.Length; i++)
    {
        lcm = Lcm(lcm, numbers[i]);
    }

    return lcm;
}

static long Lcm(long a, long b) => Math.Abs(a * b) / Gcd(a, b);

// Euclidean algorithm
static long Gcd(long a, long b)
{
    while (b != 0)
    {
        var temp = b;
        b = a % b;
        a = temp;
    }

    return a;
}