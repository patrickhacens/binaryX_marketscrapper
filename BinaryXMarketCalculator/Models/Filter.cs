namespace BinaryXMarketCalculator.Models;

public class Filter
{
    private double? _maxBNB;
    public double? MaxBNB
    {
        get => _maxBNB; 
        set
        {
            _maxBNB = value;
            _maxUSD = null;
        }
    }

    private double? _maxUSD;
    public double? MaxUSD
    {
        get => _maxUSD; 
        set
        {
            _maxUSD = value;
            _maxBNB = null;
        }
    }

    public double? MinGoldDay { get; set; }

    public int? MaxROI { get; set; }

    public FilterOrder Order { get; set; }

    public Carrer? Carrer { get; set; }

    public bool IsEmpty => !(MaxBNB.HasValue || MaxUSD.HasValue || MinGoldDay.HasValue || MaxROI.HasValue || Carrer.HasValue);

    public void Clear()
    {
        MaxBNB = null;
        MaxUSD =null;
        MinGoldDay = null;
        MaxROI = null;
        Carrer = null;
    }
}