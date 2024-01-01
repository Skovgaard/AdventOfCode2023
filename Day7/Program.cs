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

    // Part 1
    var sorted = cardsAndBid.Keys.Order(new CardsComparerClass());
    var totalWinnings = sorted.Select((hand, index) => cardsAndBid[hand] * (index + 1)).Sum();
    Console.WriteLine($"Part 1: Total winnings: {totalWinnings}");
    
    // Part 2
    var sorted2 = cardsAndBid.Keys.Order(new JokerCardsComparerClass());
    var totalWinnings2 = sorted2.Select((hand, index) => cardsAndBid[hand] * (index + 1)).Sum();
    Console.WriteLine($"Part 2: Total winnings: {totalWinnings2}");
}
catch (IOException e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
}

// Part 1
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

// Part 2
internal class JokerCardsComparerClass : Comparer<string>
{
    private const string CardValues = "AKQT98765432J";

    private const char Joker = 'J';

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
        var jokers = GetJokerCount(cardCounts);
        return jokers == 5 || cardCounts.Where(x => x.Key != Joker).Any(x => x.Value + jokers == 5);
    }

    private static bool ContainsFourOfAKind(string hand)
    {
        var cardCounts = GetCardCounts(hand);
        var jokers = GetJokerCount(cardCounts);
        return cardCounts.Where(x => x.Key != Joker).Any(x => x.Value + jokers == 4);
    }

    private static bool ContainsFullHouse(string hand)
    {
        var cardCounts = GetCardCounts(hand);
        return cardCounts.Count(x => x.Key != Joker) == 2;
    }

    private static bool ContainsThreeOfAKind(string hand)
    {
        var cardCounts = GetCardCounts(hand);
        var jokers = GetJokerCount(cardCounts);
        return cardCounts.Where(x => x.Key != Joker).Any(x => x.Value + jokers == 3);
    }

    private static bool ContainsTwoPairs(string hand)
    {
        var cardCounts = GetCardCounts(hand);
        return cardCounts.Count(x => x.Key != Joker) == 3;
    }

    private static bool ContainsOnePair(string hand)
    {
        var cardCounts = GetCardCounts(hand);
        var jokers = GetJokerCount(cardCounts);
        return cardCounts.Where(x => x.Key != Joker).Any(x => x.Value + jokers == 2);
    }

    private static int GetJokerCount(Dictionary<char, int> cardCounts)
    {
        var anyJokers = cardCounts.TryGetValue(Joker, out var jokers);
        return anyJokers ? jokers : 0;
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

    private static Type GetHandType(string hand) =>
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