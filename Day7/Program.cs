using Utilities;

var cardsAndBid = new Dictionary<string, int>();

try
{
    using var sr = Utility.ReadInputFileForProject(Directory.GetCurrentDirectory());

    while (await sr.ReadLineAsync() is { } line)
    {
        var split = line.Split(" ");
        var hand = split[0];
        var bid = int.Parse(split[1]);
        cardsAndBid.Add(hand, bid);
    }

    var sorted = cardsAndBid.Keys.Order(new CardsComparerClass());

    var totalWinnings = sorted.Select((hand, index) => cardsAndBid[hand] * (index + 1)).Sum();
    Console.WriteLine($"Total winnings: {totalWinnings}");
}
catch (IOException e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
}

internal class CardsComparerClass : Comparer<string>
{
    private const string CardValues = "AKQJT98765432";

    public override int Compare(string? x, string? y)
    {
        if (x is null || y is null)
            throw new NotSupportedException("Does not support null values");

        var xType = GetHandType(x);
        var yType = GetHandType(y);

        return xType == yType ? SecondOrderingRule(x, y) : yType.CompareTo(xType);
    }

    private static bool ContainsFiveOfAKind(string hand)
    {
        var cardCounts = GetCardCounts(hand);
        return cardCounts.ContainsValue(5);
    }

    private static bool ContainsFourOfAKind(string hand)
    {
        var cardCounts = GetCardCounts(hand);
        return cardCounts.ContainsValue(4);
    }

    private static bool ContainsFullHouse(string hand)
    {
        var cardCounts = GetCardCounts(hand);
        return cardCounts.ContainsValue(2) && cardCounts.ContainsValue(3);
    }

    private static bool ContainsThreeOfAKind(string hand)
    {
        var cardCounts = GetCardCounts(hand);
        return cardCounts.ContainsValue(3);
    }

    private static bool ContainsTwoPairs(string hand)
    {
        var cardCounts = GetCardCounts(hand);
        return cardCounts.Count(x => x.Value == 2) == 2;
    }

    private static bool ContainsOnePair(string hand)
    {
        var cardCounts = GetCardCounts(hand);
        return cardCounts.ContainsValue(2);
    }

    private static int SecondOrderingRule(string x, string y)
    {
        for (var i = 0; i < x.Length; i++)
        {
            var xValue = CardValues.IndexOf(x[i]);
            var yValue = CardValues.IndexOf(y[i]);

            if (xValue != yValue)
                return yValue.CompareTo(xValue);
        }

        return 0;
    }

    private static Dictionary<char, int> GetCardCounts(string hand)
    {
        var cardCounts = new Dictionary<char, int>();
        foreach (var card in hand)
        {
            var cardAdded = cardCounts.TryAdd(card, 1);
            if (!cardAdded)
                cardCounts[card]++;
        }

        return cardCounts;
    }

    private Type GetHandType(string hand) =>
        hand switch
        {
            _ when ContainsFiveOfAKind(hand) => Type.FiveOfAKind,
            _ when ContainsFourOfAKind(hand) => Type.FourOfAKind,
            _ when ContainsFullHouse(hand) => Type.FullHouse,
            _ when ContainsThreeOfAKind(hand) => Type.ThreeOfAKind,
            _ when ContainsTwoPairs(hand) => Type.TwoPairs,
            _ when ContainsOnePair(hand) => Type.OnePair,
            _ => Type.HighCard
        };
}

internal enum Type
{
    FiveOfAKind,
    FourOfAKind,
    FullHouse,
    ThreeOfAKind,
    TwoPairs,
    OnePair,
    HighCard
}