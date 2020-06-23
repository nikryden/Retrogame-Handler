using Newtonsoft.Json;
using RetroGameHandler.Converter;
using RetroGameHandler.Interfaces;
using System.Collections.Generic;

namespace RetroGameHandler.thegamesdbModel
{
    public class DataGames : IConvert
    {
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "games")]
        public List<Game> Games { get; set; }

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