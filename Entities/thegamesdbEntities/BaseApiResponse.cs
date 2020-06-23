using Newtonsoft.Json;
using RetroGameHandler.Converter;
using RetroGameHandler.Entities;
using RetroGameHandler.Interfaces;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroGameHandler.thegamesdbModel
{
    public class BaseApiResponse : LiteDbEntityBase
    {
        public BaseApiResponse()
        {
        }

        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "coremaining_monthly_allowancede")]
        public int RemainingMonthlyAllowance { get; set; }

        [JsonProperty(PropertyName = "extra_allowance")]
        public int ExtraAllowance { get; set; }
    }
}