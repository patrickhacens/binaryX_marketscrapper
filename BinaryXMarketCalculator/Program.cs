// requires docker run -d  --name=flaresolverr   -p 8191:8191   -e LOG_LEVEL=info   --restart unless-stopped   ghcr.io/flaresolverr/flaresolverr:latest
using BinaryXMarketCalculator;
using BinaryXMarketCalculator.Models;
using FlareSolverrSharp;
using System.Net.Http.Json;

string input;

Console.WriteLine("Starting");

MarketCalculator calculator = new();
await calculator.RefreshPrices();
await calculator.RefreshOffers();
Prices();
Console.WriteLine("Started, type help to see commands");
do
{
    input = Console.ReadLine();
    string[] param = input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(d => d.ToLower()).ToArray();
    try
    {

        switch (param[0])
        {
            case "help":
                DisplayHelp();
                break;
            case "prices":
                Prices();
                break;
            case "reprice":
                await Reprice();
                break;
            case "refresh":
                await Refresh();
                break;
            case "show":
                if (param.GetValue(1) is "filter")
                    ShowFilter();
                else
                    Show(param.GetValue(1) as string);
                break;
            case "cost":
                Cost(param.GetValue(1) as string, param.GetValue(2) as string);
                break;
            case "roi":
                ROI(param.GetValue(1) as string);
                break;
            case "carrer":
                Carrer(param.GetValue(1) as string);
                break;
            case "min":
                Min(param.GetValue(1) as string);
                break;
            case "order":
                Order(param.GetValue(1) as string);
                break;
            case "reanalise":
                calculator.Process();
                break;
            case "exit":
                break;
            default:
                Console.WriteLine("command not found, type HELP for more information on commands");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("Exception occurred");
        Console.Error.WriteLine(ex.Message);
    }
} while (input is not "exit");

void DisplayHelp()
{
    Console.WriteLine("COMMANDS\tABOUT");
    Console.WriteLine("HELP\tDisplay this text");
    Console.WriteLine("PRICES\tShow coins prices");
    Console.WriteLine("REPRICE\tReloads all prices");
    Console.WriteLine("REFRESH\tReloads offers");
    Console.WriteLine("SHOW FILTER\tShow current filter");
    Console.WriteLine("COST [NUMBER | NULL] [BNB | USD]\tFilter results by max BNB or USD");
    Console.WriteLine("ROI [NUMBER]\tFilter results by max day of ROI");
    Console.WriteLine("CARRER [CARRER]\tFilter results by carrer, values are Warrior, Thief, Mage, Ranger");
    Console.WriteLine("MIN [GOLD/DAY]\tFilter results by the min gold/day");
    Console.WriteLine("ORDER [FIELD]\tOrder results by field");
    Console.WriteLine("SHOW [number]\tShows top [number] results");
    Console.WriteLine("REANALISE\tReprocess data");
    Console.WriteLine("EXIT\tExits the application");
}

void Prices() => calculator.WritePrices();

async Task Reprice()
{
    Console.WriteLine("Refreshing prices");
    await calculator.RefreshPrices();
    Prices();
}

async Task Refresh()
{
    Console.WriteLine("Reloading all offers");
    await calculator.RefreshOffers();
    Console.WriteLine($"Offers refreshed, total of {calculator.Offers.Count} offers");
}

void ShowFilter()
{
    if (calculator.Filter.IsEmpty)
        Console.WriteLine("Current filter is empty");
    else
    {

        Console.WriteLine("Current filter:");
        if (calculator.Filter.MaxBNB.HasValue || calculator.Filter.MaxUSD.HasValue)
        {
            var (currency, value) = calculator.Filter.MaxBNB.HasValue ? ("BNB", calculator.Filter.MaxBNB) : ("USD", calculator.Filter.MaxUSD);
            Console.WriteLine($"MAX COST OF {value} {currency}");
        }

        if (calculator.Filter.MinGoldDay.HasValue)
            Console.WriteLine($"MIN G/d OF {calculator.Filter.MinGoldDay}");

        if (calculator.Filter.MaxROI.HasValue)
            Console.WriteLine($"MAX ROI OF {calculator.Filter.MaxROI} days");

        if (calculator.Filter.Carrer.HasValue)
            Console.WriteLine($"CARRER OF {calculator.Filter.Carrer}");

        Console.WriteLine($"ORDER BY {calculator.Filter.Order}");

    }
}

void Cost(string numberStr, string coinStr)
{
    if (numberStr == "null")
    {
        calculator.Filter.MaxBNB = null;
        calculator.Filter.MaxUSD = null;
        Console.WriteLine("Clearing COST filter");
        return;
    }

    if (!double.TryParse(numberStr, out var number))
    {
        Console.WriteLine("Could not parse number");
        return;
    }

    if (coinStr != "bnb" && coinStr != "usd")
    {
        Console.WriteLine("Could not parse coin");
        return;
    }
    if (coinStr is "bnb")
        calculator.Filter.MaxBNB = number;
    else if (coinStr is "usd")
        calculator.Filter.MaxUSD = number;
    else
    {
        Console.WriteLine("Could not parse command");
        return;
    }

    Console.WriteLine($"Filtering results with less than {number} {coinStr.ToUpper()}");
}

void ROI(string numberStr)
{
    if (numberStr == "null")
    {
        calculator.Filter.MaxROI = null;
        Console.WriteLine("Clearing ROI filter");
        return;
    }

    if (!int.TryParse(numberStr, out var days))
    {
        Console.WriteLine("Could not parse days");
        return;
    }

    if (days < 0)
    {
        Console.WriteLine("Days cannot be negative");
        return;
    }

    calculator.Filter.MaxROI = days;
    Console.WriteLine($"Filtering results with less than {days} of ROI");
}

void Carrer(string carrerStr)
{
    if (carrerStr == "null")
    {
        Console.WriteLine("Clearing carrer filter");
        calculator.Filter.Carrer = null;
        return;
    }

    if (!Enum.TryParse<Carrer>(carrerStr, true, out var carrer))
    {
        Console.WriteLine("Could not parse carrer");
        return;
    }

    calculator.Filter.Carrer = carrer;
    Console.WriteLine($"Filtering results of carrer {carrer}");
}

void Min(string numberStr)
{
    if (numberStr == "null")
    {
        Console.WriteLine("Clearing filter of Gold/d");
        calculator.Filter.MinGoldDay = null;
        return;
    }

    if (!double.TryParse(numberStr, out var golday))
    {
        Console.WriteLine("Could not parse gold/day");
        return;
    }

    calculator.Filter.MinGoldDay = golday;
    Console.WriteLine($"Filtering results with more than {golday} gold/day");
}

void Order(string orderStr)
{
    if (orderStr == "null")
    {
        Console.WriteLine("Clearing order");
        calculator.Filter.Order = FilterOrder.Cost;
        return;
    }

    if (!Enum.TryParse<FilterOrder>(orderStr, true, out var order))
    {
        Console.WriteLine("Could not parse order");
        Console.WriteLine("Valid input are [Gold, ROI, Cost, Net30, Net60, Net90]");
        return;
    }

    calculator.Filter.Order = order;

}

void Show(string numberStr)
{
    if (!int.TryParse(numberStr, out var number))
    {
        Console.WriteLine("Could not parse number");
        return;
    }
    calculator.Display(number);
}