using Newtonsoft.Json;
using RetroGameHandler.Converter;
using RetroGameHandler.Interfaces;

namespace RetroGameHandler.thegamesdbModel
{
    public class ImageBaseUrlMeta : IConvert
    {
        [JsonProperty(PropertyName = "original")]
        public string Original { get; set; }

        [JsonProperty(PropertyName = "small")]
        public string Small { get; set; }

        [JsonProperty(PropertyName = "thumb")]
        public string Thumb { get; set; }

        [JsonProperty(PropertyName = "cropped_center_thumb")]
        public string CroppedCenterThumb { get; set; }

        [JsonProperty(PropertyName = "medium")]
        public string Medium { get; set; }

        [JsonProperty(PropertyName = "large")]
        public string Large { get; set; }

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