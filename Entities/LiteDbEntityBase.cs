using LiteDB;
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
    public class LiteDbEntityBase : IConvert
    {
        [BsonId]
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

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