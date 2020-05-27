using Newtonsoft.Json;
using RetroGameHandler.Handlers;
using System.Collections.Generic;

namespace RetroGameHandler.thegamesAPI
{
    public class rootObj
    {
        [JsonProperty("code")]
        public int code { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("data")]
        public data data { get; set; }

        [JsonProperty("remaining_monthly_allowance")]
        public int remaining_monthly_allowance { get; set; }

        [JsonProperty("extra_allowance")]
        public int extra_allowance { get; set; }

        [JsonProperty("allowance_refresh_timer")]
        public int allowance_refresh_timer { get; set; }
    }

    public class data
    {
        [JsonProperty("count")]
        public int count { get; set; }

        [JsonProperty("platforms")]
        [JsonConverter(typeof(MyModelConverter))]
        public List<platform> platforms { get; set; }
    }

    public class platform
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("alias")]
        public string alias { get; set; }
    }
}