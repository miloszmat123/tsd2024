using GoldSavings.App.Model;
using GoldSavings.App.Client;
namespace GoldSavings.App;

public class RandomizedList<T>
{
    private List<T> list = new List<T>();
    private Random random = new Random();

    public void Add(T element)
    {
        if (random.Next(2) == 0) 
        {
            list.Insert(0, element); 
        }
        else
        {
            list.Add(element); 
        }
    }

    public T Get(int index)
    {
        if (index >= list.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }

        int randomIndex = random.Next(index + 1);
        return list[randomIndex];
    }

    public bool IsEmpty()
    {
        return list.Count == 0;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, Gold Saver!");

        GoldClient goldClient = new GoldClient();

        GoldPrice currentPrice = goldClient.GetCurrentGoldPrice().GetAwaiter().GetResult();
        Console.WriteLine($"The price for today is {currentPrice.Price}");

        List<GoldPrice> thisMonthPrices = goldClient.GetGoldPrices(new DateTime(2024, 03, 01), new DateTime(2024, 03, 11)).GetAwaiter().GetResult();
        foreach(var goldPrice in thisMonthPrices)
        {
            Console.WriteLine($"The price for {goldPrice.Date} is {goldPrice.Price}");
        }

        Console.WriteLine("Top 3 highest and lowest prices for last year using method syntax");
        var lastYearPrices = goldClient.GetGoldPrices(new DateTime(2023, 01, 01), new DateTime(2023, 12, 31)).GetAwaiter().GetResult();
        var top3HighestPrices = lastYearPrices.OrderByDescending(p => p.Price).Take(3);
        var top3LowestPrices = lastYearPrices.OrderBy(p => p.Price).Take(3);
        foreach(var goldPrice in top3HighestPrices)
        {
            Console.WriteLine($"The price for {goldPrice.Date} is {goldPrice.Price}");
        }
        foreach(var goldPrice in top3LowestPrices)
        {
            Console.WriteLine($"The price for {goldPrice.Date} is {goldPrice.Price}");
        }

        Console.WriteLine("Top 3 highest and lowest prices for last year using query syntax");
        var lastYearPricesQuery = from p in lastYearPrices
                                  select p;
        var top3HighestPricesQuery = from p in lastYearPricesQuery
                                    orderby p.Price descending
                                    select p;
        var top3LowestPricesQuery = from p in lastYearPricesQuery
                                    orderby p.Price
                                    select p;
        foreach(var goldPrice in top3HighestPricesQuery.Take(3))
        {
            Console.WriteLine($"The price for {goldPrice.Date} is {goldPrice.Price}");
        }
        foreach(var goldPrice in top3LowestPricesQuery.Take(3))
        {
            Console.WriteLine($"The price for {goldPrice.Date} is {goldPrice.Price}");
        }


        
        Console.WriteLine("\n\n\n Task 4");
        Console.WriteLine("Price from January 2020");
        var january2020Prices = goldClient.GetGoldPrices(new DateTime(2020, 01, 01), new DateTime(2020, 01, 31)).GetAwaiter().GetResult();
        Console.WriteLine("Result using method syntax");
        var resultPairsMethod = january2020Prices
        .SelectMany((startPrice, i) => january2020Prices.Skip(i + 1)
            .Select(endPrice => new { startPrice, endPrice })
            .Where(pair => (pair.endPrice.Price - pair.startPrice.Price) / pair.startPrice.Price >= 0.05))
        .Select(pair => (pair.startPrice.Date, pair.endPrice.Date))
        .ToList();
        foreach (var pair in resultPairsMethod)
        {
            Console.WriteLine($"Start Date: {pair.Item1.ToShortDateString()}, End Date: {pair.Item2.ToShortDateString()}");
        }
        Console.WriteLine("\n\n\nResult using query syntax");
        var resultPairsQuery = (from startPrice in january2020Prices.Select((value, index) => new { value, index })
                        from endPrice in january2020Prices.Skip(startPrice.index + 1)
                        where (endPrice.Price - startPrice.value.Price) / startPrice.value.Price >= 0.05
                        select (startPrice.value.Date, endPrice.Date)).ToList();

        foreach (var pair in resultPairsQuery)
        {
            Console.WriteLine($"Start Date: {pair.Item1.ToShortDateString()}, End Date: {pair.Item2.ToShortDateString()}");
        }


        DateTime startDate = new DateTime(2019, 01, 01);
        DateTime endDate = new DateTime(2022, 12, 31);
        var prices = fetchPrices(startDate, endDate, goldClient);
        
        var topPricesMethod = prices.OrderByDescending(p => p.Price)
                            .Skip(10) 
                            .Take(3) 
                            .ToList();
        Console.WriteLine("Task 5:");
        Console.WriteLine("Method Syntax Results:");
        foreach (var price in topPricesMethod)
        {
            Console.WriteLine($"Date: {price.Date.ToShortDateString()}, Price: {price.Price}");
        }

        var topPricesQuery = (from price in prices
                      orderby price.Price descending
                      select price)
                      .Skip(10) 
                      .Take(3)
                      .ToList();

        Console.WriteLine("Query Syntax Results:");
        foreach (var price in topPricesQuery)
        {
            Console.WriteLine($"Date: {price.Date.ToShortDateString()}, Price: {price.Price}");
        }

        Console.WriteLine("Task 6:");

        var prices2021 = fetchPrices(new DateTime(2021, 01, 01), new DateTime(2021, 12, 31), goldClient);

        var prices2022 = fetchPrices(new DateTime(2022, 01, 01), new DateTime(2022, 12, 31), goldClient);

        var prices2023 = fetchPrices(new DateTime(2023, 01, 01), new DateTime(2023, 12, 31), goldClient);

        //avarege price for every year using method syntax
        var averagePrice2021 = prices2021.Average(p => p.Price);
        var averagePrice2022 = prices2022.Average(p => p.Price);
        var averagePrice2023 = prices2023.Average(p => p.Price);
        Console.WriteLine("Average price for every year using method syntax");
        Console.WriteLine($"Average price for 2021: {averagePrice2021}");
        Console.WriteLine($"Average price for 2022: {averagePrice2022}");
        Console.WriteLine($"Average price for 2023: {averagePrice2023}");

        //avarege price for every year using query syntax
        var averagePrice2021Query = (from price in prices2021
                                    select price.Price).Average();
        var averagePrice2022Query = (from price in prices2022
                                    select price.Price).Average();
        var averagePrice2023Query = (from price in prices2023
                                    select price.Price).Average();
        Console.WriteLine("Average price for every year using query syntax");
        Console.WriteLine($"Average price for 2021: {averagePrice2021Query}");
        Console.WriteLine($"Average price for 2022: {averagePrice2022Query}");
        Console.WriteLine($"Average price for 2023: {averagePrice2023Query}");


        Console.WriteLine("Task 7:");
        var prices1923 = fetchPrices(new DateTime(2019, 01, 01), new DateTime(2023, 12, 31), goldClient);
        Console.WriteLine("Using method syntax");
        var minPrice1923 = prices1923.Min(p => p.Price);
        Console.WriteLine($"Min price for 2019-2023 using method syntax: {minPrice1923}");

        var maxPrice1923 = prices1923.Max(p => p.Price);
        Console.WriteLine($"Max price for 2019-2023 using method syntax: {maxPrice1923}");
        var roi = (maxPrice1923 - minPrice1923) / minPrice1923;
        Console.WriteLine($"ROI: {roi}");

        Console.WriteLine("Using query syntax");
        var minPrice1923Query = (from price in prices1923
                                select price.Price).Min();
        Console.WriteLine($"Min price for 2019-2023 using query syntax: {minPrice1923Query}");

        var maxPrice1923Query = (from price in prices1923
                                select price.Price).Max();
        Console.WriteLine($"Max price for 2019-2023 using query syntax: {maxPrice1923Query}");
        var roiQuery = (maxPrice1923Query - minPrice1923Query) / minPrice1923Query;
        Console.WriteLine($"ROI: {roiQuery}");

        savetoXML(prices1923.Take(10).ToList());
        var pricesFromXML = readFromXML();
        Console.WriteLine("Prices from XML");
        foreach (var price in pricesFromXML)
        {
            Console.WriteLine($"Date: {price.Date.ToShortDateString()}, Price: {price.Price}");
        }

        part2task1();

        RandomizedList<int> list = new RandomizedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        Console.WriteLine(list.Get(0));
        Console.WriteLine(list.Get(1));
        Console.WriteLine(list.Get(2));

        Console.WriteLine(list.IsEmpty());

    }

    static List<GoldPrice> fetchPrices(DateTime startDate, DateTime endDate, GoldClient goldClient)
    {
        var prices = new List<GoldPrice>();


        while (startDate < endDate)
        {
            var fetchEndDate = startDate.AddDays(92);
            if (fetchEndDate > endDate)
            {
                fetchEndDate = endDate;
            }

            var newPrices = goldClient.GetGoldPrices(startDate, fetchEndDate).GetAwaiter().GetResult();
            prices.AddRange(newPrices);

            startDate = startDate.AddDays(92);
            if (startDate >= endDate)
            {
                break;
            }
        }
        return prices;
    }

    static void savetoXML(List<GoldPrice> prices)
    {
        var xml = new System.Xml.Serialization.XmlSerializer(typeof(List<GoldPrice>));
        using (var stream = new System.IO.FileStream("prices.xml", System.IO.FileMode.Create))
        {
            xml.Serialize(stream, prices);
        }
    }

    static List<GoldPrice> readFromXML()
    {
        var xml = new System.Xml.Serialization.XmlSerializer(typeof(List<GoldPrice>));
        using (var stream = new System.IO.FileStream("prices.xml", System.IO.FileMode.Open))
        {
            return (List<GoldPrice>)xml.Deserialize(stream);
        }
    }

    static void part2task1(){
        Func<int, bool> isLeapYear = year => year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);

        int year = 2020;
        bool result = isLeapYear(year);
        Console.WriteLine($"{year} is a leap year: {result}");

        year = 2019;
        result = isLeapYear(year);
        Console.WriteLine($"{year} is a leap year: {result}");
    }
}
