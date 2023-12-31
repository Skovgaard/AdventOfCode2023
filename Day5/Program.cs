using Utilities;

var seeds = new List<long>();

var seedToSoilMap = new List<DataMap>();
var soilToFertilizerMap = new List<DataMap>();
var fertilizerToWaterMap = new List<DataMap>();
var waterToLightMap = new List<DataMap>();
var lightToTemperatureMap = new List<DataMap>();
var temperatureToHumidityMap = new List<DataMap>();
var humidityToLocationMap = new List<DataMap>();

long? lowestLocationNumber = null;

try
{
    using var sr = Utility.ReadInputFileForProject(Directory.GetCurrentDirectory());

    var lineNumber = 1;

    while (await sr.ReadLineAsync() is { } line)
    {
        switch (lineNumber)
        {
            case 1:
                var numbers = line.Split(":")[1].Split(" ").Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x))
                    .Select(long.Parse).ToArray();
                seeds.AddRange(numbers.ToList());
                break;
            case >= 4 and <= 20:
            {
                var data = line.Split(" ").Select(x => long.Parse(x.Trim())).ToArray();
                seedToSoilMap.Add(new DataMap(data[0], data[1], data[2]));
                break;
            }
            case >= 23 and <= 48:
            {
                var data = line.Split(" ").Select(x => long.Parse(x.Trim())).ToArray();
                soilToFertilizerMap.Add(new DataMap(data[0], data[1], data[2]));
                break;
            }
            case >= 51 and <= 97:
            {
                var data = line.Split(" ").Select(x => long.Parse(x.Trim())).ToArray();
                fertilizerToWaterMap.Add(new DataMap(data[0], data[1], data[2]));
                break;
            }
            case >= 100 and <= 107:
            {
                var data = line.Split(" ").Select(x => long.Parse(x.Trim())).ToArray();
                waterToLightMap.Add(new DataMap(data[0], data[1], data[2]));
                break;
            }
            case >= 110 and <= 124:
            {
                var data = line.Split(" ").Select(x => long.Parse(x.Trim())).ToArray();
                lightToTemperatureMap.Add(new DataMap(data[0], data[1], data[2]));
                break;
            }
            case >= 127 and <= 166:
            {
                var data = line.Split(" ").Select(x => long.Parse(x.Trim())).ToArray();
                temperatureToHumidityMap.Add(new DataMap(data[0], data[1], data[2]));
                break;
            }
            case >= 169 and <= 192:
            {
                var data = line.Split(" ").Select(x => long.Parse(x.Trim())).ToArray();
                humidityToLocationMap.Add(new DataMap(data[0], data[1], data[2]));
                break;
            }
        }

        lineNumber++;
    }

    foreach (var seed in seeds)
    {
        var soilValue = CalculateMappedValue(seedToSoilMap, seed);
        var fertilizerValue = CalculateMappedValue(soilToFertilizerMap, soilValue);
        var waterValue = CalculateMappedValue(fertilizerToWaterMap, fertilizerValue);
        var lightValue = CalculateMappedValue(waterToLightMap, waterValue);
        var temperatureValue = CalculateMappedValue(lightToTemperatureMap, lightValue);
        var humidityValue = CalculateMappedValue(temperatureToHumidityMap, temperatureValue);
        var locationValue = CalculateMappedValue(humidityToLocationMap, humidityValue);

        if (lowestLocationNumber is null || locationValue < lowestLocationNumber)
            lowestLocationNumber = locationValue;
    }

    Console.WriteLine("The lowest location number is: " + lowestLocationNumber);
}
catch (IOException e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
}

return;

long CalculateMappedValue(IEnumerable<DataMap> map, long value)
{
    var m = map.SingleOrDefault(x => value >= x.Source && value <= x.Source + x.RangeLength);
    if (m is null) return value;
    var difference = value - m.Source;
    return m.Destination + difference;
}

internal record DataMap(long Destination, long Source, long RangeLength);