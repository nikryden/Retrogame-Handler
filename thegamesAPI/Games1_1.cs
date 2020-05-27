using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace RetroGameHandler.thegamesAPI.Game1_1
{
    //[JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]

    public partial class Games
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("pages")]
        public Pages Pages { get; set; }

        [JsonProperty("remaining_monthly_allowance")]
        public long RemainingMonthlyAllowance { get; set; }

        [JsonProperty("extra_allowance")]
        public long ExtraAllowance { get; set; }

        [JsonProperty("allowance_refresh_timer")]
        public object AllowanceRefreshTimer { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("games")]
        public List<Game> Games { get; set; }
    }

    public partial class Game
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("game_title")]
        public string GameTitle { get; set; }

        [JsonProperty("release_date")]
        public DateTimeOffset ReleaseDate { get; set; }

        [JsonProperty("platform")]
        public long Platform { get; set; }

        [JsonProperty("developers")]
        public List<long> Developers { get; set; }
    }

    public partial class Pages
    {
        [JsonProperty("previous")]
        public object Previous { get; set; }

        [JsonProperty("current")]
        public Uri Current { get; set; }

        [JsonProperty("next")]
        public object Next { get; set; }
    }

    public partial class GameByName
    {
        public static GameByName FromJson(string json) => JsonConvert.DeserializeObject<GameByName>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this GameByName self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}