namespace BinaryXMarketCalculator;

class Helpers
{
    public static Dictionary<Carrer, string> ClassFilters = new()
    {
        { Carrer.Warrior, $"{nameof(Offer.Strength)},{nameof(Offer.Physique)}".ToLower() },
        { Carrer.Mage, $"{nameof(Offer.Brains)},{nameof(Offer.Charm)}".ToLower() },
        { Carrer.Thief, $"{nameof(Offer.Agility)},{nameof(Offer.Strength)}".ToLower() },
        { Carrer.Ranger, $"{nameof(Offer.Strength)},{nameof(Offer.Agility)}".ToLower() },
    };

    public static Dictionary<Carrer, string> CarrersIds = new()
    {
        { Carrer.Warrior, "0x22F3E436dF132791140571FC985Eb17Ab1846494" },
        { Carrer.Mage, "0xC6dB06fF6e97a6Dc4304f7615CdD392a9cF13F44" },
        { Carrer.Thief, "0xaF9A274c9668d68322B0dcD9043D79Cd1eBd41b3" },
        { Carrer.Ranger, "0xF31913a9C8EFE7cE7F08A1c08757C166b572a937" },
    };

    public static Dictionary<int, int> GoldForLevel = new()
    {
        { 1, 0 },
        { 2, 20000 },
        { 3, 50000 },
        { 4, 150000 },
        
    };

    public static Dictionary<string, Carrer> CarrersIdsReverse = CarrersIds.ToDictionary(d => d.Value, d => d.Key);
}
