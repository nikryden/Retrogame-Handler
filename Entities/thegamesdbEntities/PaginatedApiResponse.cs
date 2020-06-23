using Newtonsoft.Json;

namespace RetroGameHandler.thegamesdbModel
{
    public class PaginatedApiResponse : BaseApiResponse
    {
        [JsonProperty(PropertyName = "pages")]
        public Pages Pages { get; set; }
    }
}