using Newtonsoft.Json;
using RetroGameHandler.Entities.thegamesdbEntities;

namespace RetroGameHandler.thegamesdbModel
{
    public class GamesByGameID_v1 : PaginatedApiResponse
    {
        [JsonProperty(PropertyName = "data")]
        public DataGames Games { get; set; }

        [JsonProperty(PropertyName = "include")]
        public Include Include { get; set; }
    }
}