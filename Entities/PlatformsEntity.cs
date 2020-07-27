using LiteDB;
using Newtonsoft.Json;
using RetroGameHandler.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroGameHandler.Entities
{
    public class PlatformResponse
    {
        public int code { get; set; }
        public string status { get; set; }
        public data data { get; set; }
        public string remaining_monthly_allowance { get; set; }
        public int extra_allowance { get; set; }
        public long allowance_refresh_timer { get; set; }
    }

    public class data
    {
        public int Count { get; set; }
        public List<PlatformEntity> Platforms { get; set; }
    }

    public class PlatformEntity : LiteDbEntityBase, ICloneable
    {
        public PlatformEntity()
        {
        }

        [BsonCtor]
        public PlatformEntity(int id,
            string name,
            string alias,
            string console,
            string controller,
            string developer,
            string icon,
            string overview
            )
        {
            Id = id;
            Icon = icon;
            Name = name;
            Overview = overview;
            Developer = developer;
            Controller = controller;
            Console = console;
            Alias = alias;
        }

        public PlatformEntity(PlatformModel PlatformModel)
        {
            var Properties = PlatformModel.GetType().GetProperties();
            foreach (var propertyInfo in Properties)
            {
                var prop = this.GetType().GetProperty(propertyInfo.Name);
                if (prop != null) prop.SetValue(this, propertyInfo.GetValue(PlatformModel));
            }
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "alias")]
        public string Alias { get; set; }

        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; set; }

        [JsonProperty(PropertyName = "console")]
        public string Console { get; set; }

        [JsonProperty(PropertyName = "controller")]
        public string Controller { get; set; }

        [JsonProperty(PropertyName = "developer")]
        public string Developer { get; set; }

        [JsonProperty(PropertyName = "overview")]
        public string Overview { get; set; }

        [JsonProperty(PropertyName = "manufacturer")]
        public string Manufacturer { get; set; }

        [JsonProperty(PropertyName = "media")]
        public string Media { get; set; }

        [JsonProperty(PropertyName = "cpu")]
        public string Cpu { get; set; }

        [JsonProperty(PropertyName = "memory")]
        public string Memory { get; set; }

        [JsonProperty(PropertyName = "graphics")]
        public string Graphics { get; set; }

        [JsonProperty(PropertyName = "sound")]
        public string Sound { get; set; }

        [JsonProperty(PropertyName = "maxcontrollers")]
        public string Maxcontrollers { get; set; }

        [JsonProperty(PropertyName = "display")]
        public string Display { get; set; }

        [JsonProperty(PropertyName = "youtube")]
        public string Youtube { get; set; }

        [JsonProperty(PropertyName = "extensions")]
        public List<string> Extensions { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}