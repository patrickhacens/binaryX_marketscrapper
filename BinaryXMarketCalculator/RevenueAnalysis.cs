namespace BinaryXMarketCalculator
{
    public class RevenueAnalysis
    {
        public Opportunity Opportunity { get; set; }

        public decimal TotalInvesment { get; set; }

        public decimal TotalInvesmentInBnb { get; set; }

        public decimal BnxCost { get; set; }

        public decimal GoldCost { get; set; }

        public decimal LevelUpCost { get; set; }

        public int LevelUpBy { get; set; }

        public TimeSpan Roi { get; set; }
    }
}
