using System.Text.Json;
using System.Text.Json.Serialization;

namespace BinaryXMarketCalculator.Models.Converters;

public class CoinConverter : JsonConverter<CoinType>
{
    public override CoinType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        return str switch
        {
            "" => CoinType.BNB,
            "0x8C851d1a123Ff703BD1f9dabe631b69902Df5f97" => CoinType.BNX,
            "0xb3a6381070b1a15169dea646166ec0699fdaea79" => CoinType.Gold,
            _ => throw new NotImplementedException(),
        };
    }

    public override void Write(Utf8JsonWriter writer, CoinType value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
