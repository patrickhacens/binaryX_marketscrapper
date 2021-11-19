using BinaryXMarketCalculator.Models;
using BinaryXMarketCalculator.Models.Converters;
using FlareSolverrSharp;
using System.Net.Http.Json;
using System.Text.Json;

namespace BinaryXMarketCalculator
{
    public class MarketCalculator
    {
        public int BlocksPerDay { get; set; } = 57600;
        public HttpClient Client { get; init; }
        public double BNBPrice { get; set; }
        public double GoldPrice { get; set; }
        public double BNXPrice { get; set; }

        public Filter Filter { get; set; }
        public List<Offer> Offers { get; set; }

        public List<RevenueAnalysis> Analysis { get; set; }

        private bool hasNewData = true;

        private readonly JsonSerializerOptions _jsonOp;

        private readonly static Dictionary<int, int> GoldForLevel = new()
        {
            { 1, 0 },
            { 2, 20000 },
            { 3, 50000 },
            { 4, 150000 },
        };


        public MarketCalculator()
        {
            Filter = new Filter();
            Offers = new List<Offer>();

            _jsonOp = new()
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new CoinConverter(),
                    new CarrerConverter()
                }
            };

            var handler = new ClearanceHandler("http://localhost:8191");
            Client = new(handler)
            {
                DefaultRequestHeaders =
                {
                    { "Accept", "application/json" }
                }
            };
        }

        public async Task RefreshPrices()
        {
            var bnx = GetBnxPrice();
            var gold = GetGoldPrice();
            var bnb = GetBnbPrice();

            await Task.WhenAll(bnx, gold, bnb);

            BNXPrice = double.Parse(bnx.Result.Data.Price);
            GoldPrice = double.Parse(gold.Result.Data.Price);
            BNBPrice = double.Parse(bnb.Result.Price);
        }

        public void WritePrices() => Console.WriteLine($"BNB: {BNBPrice}\tBNX: {BNXPrice}\tGold: {GoldPrice}");


        public void ClearFilter() => Filter.Clear();

        public void WriteHeaders() =>
            Console.WriteLine(TabStr(
                "TOTAL COST",
                "ROI",
                "GOLD/Day",
                "LEVEL",
                "JOB",
                "LINK"));

        public void WriteOffer(RevenueAnalysis o) =>
            Console.WriteLine(TabStr(
                $"USD {o.TotalInvestment:N2} | {o.TotalInvestmentInBnb:N4} BNB",
                $"{o.Roi.TotalDays:N1}",
                $"{o.GoldDay:N0}G/d",
                $"{o.Offer.Level} => {o.Offer.Level + o.LevelUpBy} ({o.LevelUpCost/1000}kG)",
                $"{o.Job}",
                $"https://market.binaryx.pro/#/oneoffsale/detail/{o.Offer.Order_Id}"
                ));

        public async Task RefreshOffers()
        {
            List<Offer> result = new();

            foreach (var filter in Helpers.ClassFilters)
            {
                int page = 0;
                int count = -1;
                int size = 99;

                while (count == -1 || page * size < count)
                {
                    var response = await Client.GetFromJsonAsync<BinaryXMartketApiResult>($"https://market.binaryx.pro/info/getSales?page={page}&page_size={size}&status=selling&n&sort=price&direction=asc&career={Helpers.CarrersIds[filter.Key]}&value_attr={filter.Value}&start_value=86,61&end_value=0,0&pay_addr=", _jsonOp);

                    if (count == -1)
                        count = response?.Data?.Result?.Total ?? 0;

                    if (response?.Data?.Result?.Items?.Any() == true)
                        result.AddRange(response.Data.Result.Items);

                    page++;
                }
            }
            Offers = result;
        }

        public void Process()
        {
            if (Offers?.Any() == false)
            {
                Console.WriteLine("There are no offers to analyse");
                return;
            }
            Analysis = Offers
                .Select(offer => Enumerable
                    .Range(0, Math.Max(0, 5 - offer.Level))
                    .Select(pLvl => GetJobsFor(offer)
                        .Select(job => Analyze(offer, offer.Level + pLvl, job))))
                .SelectMany(d => d)
                .SelectMany(d => d)
                .ToList();
        }

        public void Display(int number)
        {
            if (hasNewData)
            {
                Console.WriteLine("Datasource is new, analysing");
                hasNewData = false;
                Process();
            }

            if (Analysis?.Any() == false)
            {
                Console.WriteLine("Datasource is empty");
                return;
            }
            var query = Analysis.AsEnumerable();

            if (Filter.MaxBNB.HasValue)
                query = query.Where(d => d.TotalInvestmentInBnb <= Filter.MaxBNB.Value);

            if (Filter.MaxUSD.HasValue)
                query = query.Where(d => d.TotalInvestment <= Filter.MaxUSD.Value);

            if (Filter.Carrer.HasValue)
                query = query.Where(d => d.Offer.Carrer == Filter.Carrer.Value);

            if (Filter.MaxROI.HasValue)
                query = query.Where(d => d.Roi.Days <= Filter.MaxROI.Value);

            if (Filter.MinGoldDay.HasValue)
                query = query.Where(d => d.GoldDay >= Filter.MinGoldDay.Value);

            switch (Filter.Order)
            {
                case FilterOrder.Gold:
                    query = query.OrderByDescending(d => d.GoldDay).ThenBy(d => d.Roi).ThenBy(d => d.TotalInvestment);
                    break;
                case FilterOrder.ROI:
                    query = query.OrderBy(d => d.Roi).ThenByDescending(d => d.GoldDay).ThenBy(d => d.TotalInvestment);
                    break;
                case FilterOrder.Cost:
                    query = query.OrderBy(d => d.TotalInvestment).ThenByDescending(d => d.GoldDay).ThenBy(d => d.Roi);
                    break;
                default:
                    break;
            }

            WriteHeaders();
            foreach (var offer in query.Take(number))
                WriteOffer(offer);
        }

        private string TabStr(params string[] strs) => String.Join("\t", strs);

        private Task<CoinApiResult> GetBnxPrice()
            => Client.GetFromJsonAsync<CoinApiResult>("https://api.pancakeswap.info/api/v2/tokens/0x8C851d1a123Ff703BD1f9dabe631b69902Df5f97", _jsonOp);

        private Task<CoinApiResult> GetGoldPrice()
            => Client.GetFromJsonAsync<CoinApiResult>("https://api.pancakeswap.info/api/v2/tokens/0xb3a6381070b1a15169dea646166ec0699fdaea79", _jsonOp);

        private Task<BinanceApiResult> GetBnbPrice()
            => Client.GetFromJsonAsync<BinanceApiResult>("https://api.binance.com/api/v3/avgPrice?symbol=BNBUSDT", _jsonOp);

        private double GoldByDay(int level, int strength, int agility, int physique, int volition, int brains, int charm, GoldJob job)
        {
            var salaryByBlock = job switch
            {
                GoldJob.PartTime => 0.01,
                GoldJob.Hunting => SalaryByBlockBasicDungeon(strength),
                GoldJob.Lumberjack => SalaryByBlockBasicDungeon(strength),
                GoldJob.ScrollScribe => SalaryByBlockBasicDungeon(brains),
                GoldJob.Winemaker => SalaryByBlockBasicDungeon(agility),
                GoldJob.LegendaryField => 0.065 + (strength+agility+physique+volition+brains+charm-401)*0.0025,
                _ => 0,
            };
            var salaryPerDay = salaryByBlock * BlocksPerDay;
            return salaryPerDay * Math.Pow(2, level-2);
        }

        private IEnumerable<GoldJob> GetJobsFor(Offer offer)
        {
            if (offer.Carrer == Carrer.Mage && offer is { Brains: > 86, Charm: > 61 })
                yield return GoldJob.ScrollScribe;

            if (offer.Carrer == Carrer.Ranger && offer is { Strength: > 86, Agility: > 61 })
                yield return GoldJob.Hunting;

            if (offer.Carrer == Carrer.Thief && offer is { Agility: > 86, Strength: > 61 })
                yield return GoldJob.Winemaker;

            if (offer.Carrer == Carrer.Warrior && offer is { Strength: > 86, Physique: > 61 })
                yield return GoldJob.Lumberjack;

            if (offer.Total > 401)
                yield return GoldJob.LegendaryField;
        }

        private RevenueAnalysis Analyze(Offer offer, int level, GoldJob job)
        {
            double offerPrice = double.Parse(offer.Price);
            var goldByDay = GoldByDay(level, offer.Strength, offer.Agility, offer.Physique, offer.Volition, offer.Brains, offer.Charm, job);
            var goldCost = offer.Coin == CoinType.Gold ? offerPrice : 0;
            var bnxCost = offer.Coin == CoinType.BNX ? offerPrice / Math.Pow(10, 18) : 0;
            var levelUpBy = level - offer.Level;
            var lvlUpCost = GetLevelUpCost(offer.Level, level);
            var totalInvestment = lvlUpCost * GoldPrice + goldCost * GoldPrice + bnxCost * BNXPrice;
            var totalInvestmentInBnb = totalInvestment / BNBPrice;
            var roi = TimeSpan.FromHours((double)(totalInvestment / (goldByDay * GoldPrice)) * 24);
            return new()
            {
                Offer = offer,
                BnxCost = bnxCost,
                GoldCost = goldCost,
                GoldDay = goldByDay,
                LevelUpBy = levelUpBy,
                LevelUpCost = lvlUpCost,
                Job = job,
                Roi = roi,
                TotalInvestment = totalInvestment,
                TotalInvestmentInBnb = totalInvestmentInBnb,
            };
        }

        private static double SalaryByBlockBasicDungeon(int mainStat)
            => (0.01 + (mainStat - 85) * 0.005);

        private static double GetLevelUpCost(int currentLevel, int targetLevel)
        {
            double total = 0;
            for (int i = currentLevel; i < targetLevel; i++)
                total += GoldForLevel[i+1];

            return total;
        }

    }
}
