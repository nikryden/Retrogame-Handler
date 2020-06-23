using Newtonsoft.Json;
using RetroGameHandler.Converter;
using RetroGameHandler.Interfaces;

namespace RetroGameHandler.thegamesdbModel
{
    public class Pages : IConvert
    {
        [JsonProperty(PropertyName = "previous")]
        public string previous { get; set; }

        [JsonProperty(PropertyName = "current")]
        public string Current { get; set; }

        [JsonProperty(PropertyName = "next")]
        public string Next { get; set; }

        public void Convert(object obj)
        {
            ObjectConverter.Convert(this, obj);
        }

        public void ConvertBack(object obj)
        {
            ObjectConverter.Convert(obj, this);
        }
    }
}