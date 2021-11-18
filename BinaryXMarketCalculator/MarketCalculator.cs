using BinaryXMarketCalculator.Models.Converters;
using FlareSolverrSharp;
using System.Net.Http.Json;
using System.Text.Json;

namespace BinaryXMarketCalculator
{
    public class MarketCalculator
    {
        public string MarketAddress { get; set; }

        public string PancakeAddress { get; set; }

        public string BinanceAddress { get; set; }

        public HttpClient Client { get; init; }


        public decimal BNBPrice { get; set; }
        public decimal GoldPrice { get; set; }
        public decimal BNXPrice { get; set; }

        private readonly JsonSerializerOptions _jsonOp;

        public MarketCalculator()
        {
            MarketAddress = "";
            PancakeAddress = "";
            BinanceAddress = "";

            _jsonOp = new()
            {
                Converters =
                {
                    new CoinConverter(),
                    new CarrerConverter()
                }
            };

            var handler = new ClearanceHandler("http://localhost:8191");
            Client = new(handler);
        }

        public async Task RefreshPrices()
        {
            var bnx = GetBnxPrice();
            var gold = GetGoldPrice();
            var bnb = GetBnbPrice();

            await Task.WhenAll(bnx, gold, bnb);

            BNXPrice = bnx.Result.Data.Price;
            GoldPrice = gold.Result.Data.Price;
            BNBPrice = bnb.Result.Price;
        }

        private Task<CoinApiResult> GetBnxPrice()
            => Client.GetFromJsonAsync<CoinApiResult>("https://api.pancakeswap.info/api/v2/tokens/0x8C851d1a123Ff703BD1f9dabe631b69902Df5f97", _jsonOp);

        private Task<CoinApiResult> GetGoldPrice()
            => Client.GetFromJsonAsync<CoinApiResult>("https://api.pancakeswap.info/api/v2/tokens/0xb3a6381070b1a15169dea646166ec0699fdaea79", _jsonOp);

        private Task<BinanceApiResult> GetBnbPrice()
            => Client.GetFromJsonAsync<BinanceApiResult>("https://api.binance.com/api/v3/avgPrice?symbol=BNBUSDT", _jsonOp);


        private Task<>

        private static int GetMainStat(Offer item) => Helpers.CarrersIdsReverse[item.Career_Address] switch
        {
            Carrer.Mage => item.Brains,
            Carrer.Ranger => item.Strength,
            Carrer.Thief => item.Agility,
            Carrer.Warrior => item.Strength,
            _ => -1
        };
    }
}
