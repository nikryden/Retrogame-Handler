using Newtonsoft.Json;
using RetroGameHandler.thegamesdbModel;

namespace RetroGameHandler.Entities.thegamesdbEntities
{
    public class Include
    {
        [JsonProperty(PropertyName = "boxart")]
        public Boxart BoxArt { get; set; }
    }
}