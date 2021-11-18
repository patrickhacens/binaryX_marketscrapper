using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BinaryXMarketCalculator.Models.Converters
{
    public class CarrerConverter : JsonConverter<Carrer>
    {
        public override Carrer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();

            return str switch
            {
                "0x22F3E436dF132791140571FC985Eb17Ab1846494" => Carrer.Warrior,
                "0xC6dB06fF6e97a6Dc4304f7615CdD392a9cF13F44" => Carrer.Mage,
                "0xaF9A274c9668d68322B0dcD9043D79Cd1eBd41b3" => Carrer.Thief,
                "0xF31913a9C8EFE7cE7F08A1c08757C166b572a937" => Carrer.Ranger,
                _ => Carrer.Unknown
            };
        }

        public override void Write(Utf8JsonWriter writer, Carrer value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
