using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryXMarketCalculator;

public class ApiResult
{
    public int Code { get; set; }
    public Data Data { get; set; }
}

public class Data
{
    public Result Result { get; set; }
    public object Error { get; set; }
}

public class Result
{
    public int Total { get; set; }
    public Item[] Items { get; set; }
}

public class Item
{
    public string Order_Id { get; set; }
    public string Name { get; set; }
    public string Price { get; set; }
    public string Buyer { get; set; }
    public string Seller { get; set; }
    public string Pay_Addr { get; set; }
    public string Career_Address { get; set; }
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

public enum Carrer
{
    Warrior,
    Thief,
    Mage,
    Ranger,
}


public class CoinApiResult
{
    public long Updated_At { get; set; }
    public CoinApiData Data { get; set; }
}

public class CoinApiData
{
    public string Name { get; set; }
    public string Symbol { get; set; }
    public decimal Price { get; set; }
    public decimal Price_BNB { get; set; }
}



public class BinanceApiResult
{
    public int Mins { get; set; }
    public decimal Price { get; set; }
}


public class RoiResult
{
    public Item Item { get; set; }
    public int Level { get; set; }
    public int LevelUpBy { get; set; }
    public decimal GoldByDay { get; set; }
    public decimal MinInvestInGold { get; set; }
    public decimal InvestmentInGold { get; set; }
    public decimal MinInvestInBnx { get; set; }
    public decimal TotalInvestment { get; set; }

    public decimal TotalInvestmentInBnb { get; set; }
    public TimeSpan Roi { get; set; }
}


