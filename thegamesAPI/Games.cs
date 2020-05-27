namespace RetroGameHandler.thegamesAPI.Games
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using RetroGameHandler.Handlers;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public partial class Games
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("data")]
        public GamesData Data { get; set; }

        [JsonProperty("include")]
        public Include Include { get; set; }

        [JsonProperty("pages")]
        public Pages Pages { get; set; }

        [JsonProperty("remaining_monthly_allowance")]
        public long RemainingMonthlyAllowance { get; set; }

        [JsonProperty("extra_allowance")]
        public long ExtraAllowance { get; set; }

        [JsonProperty("allowance_refresh_timer")]
        public object AllowanceRefreshTimer { get; set; }
    }

    public partial class GamesData
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

    public partial class Include
    {
        [JsonProperty("boxart")]
        public Boxart Boxarts { get; set; }

        [JsonProperty("platform")]
        public Platforms Platform { get; set; }
    }

    public partial class Boxart
    {
        [JsonProperty("base_url")]
        public BaseUrl BaseUrl { get; set; }

        [JsonProperty("data")]
        public Dictionary<long, List<data>> data { get; set; }
    }

    public partial class BaseUrl
    {
        [JsonProperty("original")]
        public Uri Original { get; set; }

        [JsonProperty("small")]
        public Uri Small { get; set; }

        [JsonProperty("thumb")]
        public Uri Thumb { get; set; }

        [JsonProperty("cropped_center_thumb")]
        public Uri CroppedCenterThumb { get; set; }

        [JsonProperty("medium")]
        public Uri Medium { get; set; }

        [JsonProperty("large")]
        public Uri Large { get; set; }
    }

    public partial class data
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("resolution")]
        public string Resolution { get; set; }
    }

    public partial class Platforms
    {
        [JsonProperty("data")]
        [JsonConverter(typeof(MyModelConverter))]
        public List<Platform> Data { get; set; }
    }

    public partial class Platform
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }
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

    public partial class Games
    {
        public static Games FromJson(string json) => JsonConvert.DeserializeObject<Games>(json, RetroGameHandler.thegamesAPI.Games.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Games self) => JsonConvert.SerializeObject(self, RetroGameHandler.thegamesAPI.Games.Converter.Settings);
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