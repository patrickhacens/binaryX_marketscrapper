using BinaryXMarketCalculator;
using FlareSolverrSharp;
using System.Net.Http.Json;

// requires docker run -d  --name=flaresolverr   -p 8191:8191   -e LOG_LEVEL=info   --restart unless-stopped   ghcr.io/flaresolverr/flaresolverr:latest



var bnbPriceResponse = await R.GetBnbPrice();
var bnxPriceResponse = await R.GetBnxPrice();
var goldPriceResponse = await R.GetGoldPrice();


var bnxPrice = bnxPriceResponse.Data.Price;
var goldPrice = goldPriceResponse.Data.Price;
var bnbPrice = bnbPriceResponse.Price;

Console.WriteLine($"BNB: {bnbPrice}\tBNX: {bnxPrice}\t GOLD: {goldPrice}");

Console.WriteLine("Loading offers");
var itens = await R.LoadAllViableItens();

RoiResult GetResultForLevel(Offer item, int level)
{
    int mainStat = R.GetMainStat(item);
    var goldByDay = R.GoldByDay(mainStat, level);
    var minInvestInGold = R.GetGoldToLevelUp(item.Level, level);
    var levelUpBy = level - item.Level;
    var investmentInGold = minInvestInGold * goldPrice;
    var minInvestInBnx = (decimal)(double.Parse(item.Price) / Math.Pow(10, 18));
    var totalInvestment = minInvestInBnx * bnxPrice + investmentInGold;
    var totalInvestmentInBnb = totalInvestment / bnbPrice;
    var roi = TimeSpan.FromHours((double)(totalInvestment / (goldByDay * goldPrice)) * 24);
    var result = new RoiResult()
    {
        Item = item,
        Level = level,
        MinInvestInBnx = minInvestInBnx,
        GoldByDay = goldByDay,
        LevelUpBy = levelUpBy,
        TotalInvestment = totalInvestment,
        InvestmentInGold = investmentInGold,
        MinInvestInGold = minInvestInGold,
        TotalInvestmentInBnb = totalInvestmentInBnb,
        Roi = roi,
    };
    return result;
}


var data = itens.Select(item =>
{
    List<RoiResult> results = new List<RoiResult>();
    for (int i = Math.Max(2,item.Level); i <= 4; i++)
        results.Add(GetResultForLevel(item, i));
    return results;
})
    .SelectMany(d => d)
    .ToList();







var query = data.AsEnumerable();
var minBnb = data.Min(d => d.TotalInvestmentInBnb);
Console.Write($"Minimum BNB for acquisition is {minBnb}");


Console.WriteLine("Filter by total acquisition in bnb? if yes write value else press enter");

string input = Console.ReadLine();

decimal bnb = 0;
bool filter = decimal.TryParse(input, out bnb);

if (filter)
    query = query.Where(d => d.TotalInvestmentInBnb <= bnb);

var results = query
    .OrderByDescending(d => d.GoldByDay)
    .ThenBy(d => d.TotalInvestment)
    .ToList();


if (results.Any())
{
    Console.WriteLine("INVESTMENTO\tGOLD/D\tROI\tMAINSTAT\tTARGET LVL\tLINK");
    foreach (var item in results.Take(60))
    {
        Console.WriteLine($"Inv: USD {item.TotalInvestment:N2} | {item.TotalInvestmentInBnb:N6} BNB \t {item.GoldByDay} gold/d | {item.GoldByDay * goldPrice:N2}USD/d \t ROI em {item.Roi.TotalDays:N1} dias \t mainstat {R.GetMainStat(item.Item)} \t lvl {item.Level} (+{item.LevelUpBy} | {item.MinInvestInGold} gold) \t https://market.binaryx.pro/#/oneoffsale/detail/{item.Item.Order_Id}");
    }
}
else
    Console.WriteLine("There are no results");


Console.ReadLine();

public static class R
{


    public static HttpClient _client = new HttpClient()
    {
        DefaultRequestHeaders =
        {
            { "Accept", "application/json" }
        }
    };



    public static decimal GoldByDay(int mainStat, int level)
        => (decimal)((864 + 288 * (mainStat - 86)) * Math.Pow(2, level - 2));


    

    public static Task<CoinApiResult> GetBnxPrice()
        => _client.GetFromJsonAsync<CoinApiResult>("https://api.pancakeswap.info/api/v2/tokens/0x8C851d1a123Ff703BD1f9dabe631b69902Df5f97");

    public static Task<CoinApiResult> GetGoldPrice()
        => _client.GetFromJsonAsync<CoinApiResult>("https://api.pancakeswap.info/api/v2/tokens/0xb3a6381070b1a15169dea646166ec0699fdaea79");

    public static Task<BinanceApiResult> GetBnbPrice() 
        => _client.GetFromJsonAsync<BinanceApiResult>("https://api.binance.com/api/v3/avgPrice?symbol=BNBUSDT");


    public static async Task<List<Offer>> LoadAllViableItens()
    {
        var handler = new ClearanceHandler("http://localhost:8191");

        HttpClient client = new(handler)
        {
            BaseAddress = new Uri("https://market.binaryx.pro")
        };

        List<Offer> result = new();

        foreach (var filter in Helpers.ClassFilters)
        {

            int page = 0;
            int count = -1;
            int size = 99;

            while (count == -1 || page * size < count)
            {
                var response = await client.GetFromJsonAsync<BinaryXMartketApiResult>($"info/getSales?page={page}&page_size={size}&status=selling&n&sort=price&direction=asc&career={Helpers.CarrersIds[filter.Key]}&value_attr={filter.Value}&start_value=86,61&end_value=0,0&pay_addr=");

                if (count == -1)
                    count = response?.Data?.Result?.Total ?? 0;

                if (response?.Data?.Result?.Items?.Any() == true)
                    result.AddRange(response.Data.Result.Items);

                page++;
            }
        }
        return result;
    }

    public static int GetGoldToLevelUp(int currentLevel, int targetLevel)
    {
        int result = 0;
        while (currentLevel < targetLevel)
            result += Helpers.GoldForLevel[++currentLevel];
        return result;
    }



}


