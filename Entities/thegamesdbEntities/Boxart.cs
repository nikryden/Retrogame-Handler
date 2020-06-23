using Newtonsoft.Json;
using RetroGameHandler.Converter;
using RetroGameHandler.Interfaces;
using System.Collections.Generic;

namespace RetroGameHandler.thegamesdbModel
{
    public class Boxart : IConvert
    {
        [JsonProperty(PropertyName = "base_url")]
        public ImageBaseUrlMeta BaseUrls { get; set; }

        [JsonProperty(PropertyName = "data")]
        public Dictionary<string, List<GameImage>> GameImages { get; set; }

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