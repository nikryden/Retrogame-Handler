using Newtonsoft.Json;
using RetroGameHandler.Converter;
using RetroGameHandler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroGameHandler.Entities
{
    internal class GameExtensions : IConvert
    {
        [JsonProperty(PropertyName = "emulators")]
        public List<Emulators> Emulators { get; set; }

        public void Convert(object obj)
        {
            ObjectConverter.Convert(this, obj);
        }

        public void ConvertBack(object obj)
        {
            ObjectConverter.Convert(obj, this);
        }
    }

    internal class Emulators : IConvert
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "extensions")]
        public List<string> Extensions { get; set; }

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