using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroGameHandler.Entities
{
    public class EmuExtensionsEntity
    {
        public EmuExtensionsEntity()
        {
        }

        [JsonProperty(PropertyName = "emulators")]
        public List<EmuExtensionEntity> Extensions { get; set; }
    }

    public class EmuExtensionEntity
    {
        public EmuExtensionEntity()
        {
        }

        [JsonProperty(PropertyName = "id")]
        public int id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "extensions")]
        public List<string> extensoins { get; set; }
    }
}