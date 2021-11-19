namespace BinaryXMarketCalculator
{
    public class RevenueAnalysis
    {
        public Offer Offer { get; set; }
        public GoldJob Job { get; set; }
        public double TotalInvestment { get; set; }
        public double TotalInvestmentInBnb { get; set; }
        public double BnxCost { get; set; }
        public double GoldCost { get; set; }
        public double GoldDay { get; set; }
        public double LevelUpCost { get; set; }
        public int LevelUpBy { get; set; }
        public TimeSpan Roi { get; set; }
    }
}
