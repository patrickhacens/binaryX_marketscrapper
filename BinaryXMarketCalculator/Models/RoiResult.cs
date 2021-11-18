namespace BinaryXMarketCalculator;

public class RoiResult
{
    public Offer Item { get; set; }
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


