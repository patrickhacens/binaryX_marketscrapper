using BinaryXMarketCalculator.Models;
using System.Text.Json.Serialization;

namespace BinaryXMarketCalculator;

public class Offer
{
    public string Order_Id { get; set; }
    public string Name { get; set; }
    public string Price { get; set; }
    public string Buyer { get; set; }
    public string Seller { get; set; }

    [JsonPropertyName("pay_addr")]
    public CoinType Coin { get; set; }

    [JsonPropertyName("career_address")]
    public Carrer Carrer { get; set; }
    public string Token_Id { get; set; }
    public int Block_Number { get; set; }
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int Physique { get; set; }
    public int Volition { get; set; }
    public int Brains { get; set; }
    public int Charm { get; set; }
    public int Total { get; set; }
    public int Level { get; set; }
}


