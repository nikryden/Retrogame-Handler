using Newtonsoft.Json;
using RetroGameHandler.Converter;
using RetroGameHandler.Interfaces;

namespace RetroGameHandler.thegamesdbModel
{
    public class Game : IConvert
    {
        [JsonProperty(PropertyName = "id")]
        public int id { get; set; }

        [JsonProperty(PropertyName = "game_title")]
        public string GameTitle { get; set; }

        [JsonProperty(PropertyName = " release_date")]
        public string ReleaseDate { get; set; }

        [JsonProperty(PropertyName = "platform")]
        public int Platform { get; set; }

        [JsonProperty(PropertyName = "players")]
        public int? Players { get; set; }

        [JsonProperty(PropertyName = "overview")]
        public string Overview { get; set; }

        [JsonProperty(PropertyName = "last_updated")]
        public string LastUpdated { get; set; }

        [JsonProperty(PropertyName = "rating")]
        public string Rating { get; set; }

        [JsonProperty(PropertyName = "coop")]
        public string Coop { get; set; }

        [JsonProperty(PropertyName = "youtube")]
        public string Youtube { get; set; }

        [JsonProperty(PropertyName = "os")]
        public string Os { get; set; }

        [JsonProperty(PropertyName = "processor")]
        public string Processor { get; set; }

        [JsonProperty(PropertyName = "ram")]
        public string Ram { get; set; }

        [JsonProperty(PropertyName = "hdd")]
        public string HDD { get; set; }

        [JsonProperty(PropertyName = "video")]
        public string Video { get; set; }

        [JsonProperty(PropertyName = "sound")]
        public string Sound { get; set; }

        [JsonProperty(PropertyName = "developers")]
        public int[] Developers { get; set; }

        [JsonProperty(PropertyName = "genres")]
        public int[] Genres { get; set; }

        [JsonProperty(PropertyName = "publishers")]
        public int[] Publishers { get; set; }

        [JsonProperty(PropertyName = "alternates")]
        public string[] Alternates { get; set; }

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