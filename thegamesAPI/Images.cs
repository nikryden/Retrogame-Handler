using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RetroGameHandler.Handlers;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace RetroGameHandler.thegamesAPI.Image
{
    public partial class Images
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

        [JsonProperty("base_url")]
        public BaseUrl BaseUrl { get; set; }

        [JsonProperty("images")]
        [JsonConverter(typeof(MyModelConverter))]
        public List<Image> Images { get; set; }
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

    //public partial class ImagesClass
    //{
    //    [JsonProperty("Images")]

    //    public List<Image> Images { get; set; }
    //}

    public partial class Image
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

    public partial class Pages
    {
        [JsonProperty("previous")]
        public object Previous { get; set; }

        [JsonProperty("current")]
        public Uri Current { get; set; }

        [JsonProperty("next")]
        public object Next { get; set; }
    }

    public partial class Images
    {
        public static Images FromJson(string json) => JsonConvert.DeserializeObject<Images>(json, RetroGameHandler.thegamesAPI.Image.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Images self) => JsonConvert.SerializeObject(self, RetroGameHandler.thegamesAPI.Image.Converter.Settings);
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