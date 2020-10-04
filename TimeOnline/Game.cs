using Newtonsoft.Json;
using System.Xml;
using System.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroGameHandler.TimeOnline
{
    public class Info
    {
        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("error")]
        public Error Error { get; set; }
    }

    public class GameRoot
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("games")]
        public List<Game> Games { get; set; }

        [JsonProperty("base_url")]
        public List<BaseUrl> BaseUrls { get; set; }

        [JsonProperty("userinfo")]
        public UserInfo UserInfo { get; set; }

        [JsonProperty("error")]
        public Error Error { get; set; }
    }

    public class Error
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class Game
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("game_title")]
        public string GameTitle { get; set; }

        [JsonProperty("release_date")]
        public string release_date { get; set; }

        [JsonProperty("platform")]
        public int platform { get; set; }

        [JsonProperty("overview")]
        public string overview { get; set; }

        [JsonProperty("boxart")]
        public List<BoxArt> BoxArts { get; set; }

        //NEW********************not in use
        [JsonProperty("SOUNDEX")]
        public string SOUNDEX { get; set; }

        [JsonProperty("players")]
        public string players { get; set; }

        [JsonProperty("publisher_to_be_removed")]
        public string publisher_to_be_removed { get; set; }

        [JsonProperty("last_updated")]
        public string last_updated { get; set; }

        [JsonProperty("rating")]
        public string rating { get; set; }

        [JsonProperty("hits")]
        public string hits { get; set; }

        [JsonProperty("mirrorupdate")]
        public string mirrorupdate { get; set; }

        [JsonProperty("disabled")]
        public string disabled { get; set; }

        [JsonProperty("coop")]
        public string coop { get; set; }

        [JsonProperty("youtube")]
        public string youtube { get; set; }

        [JsonProperty("os")]
        public string os { get; set; }

        [JsonProperty("processor")]
        public string processor { get; set; }

        [JsonProperty("ram")]
        public string ram { get; set; }

        [JsonProperty("hdd")]
        public string hdd { get; set; }

        [JsonProperty("video")]
        public string video { get; set; }

        [JsonProperty("sound")]
        public string sound { get; set; }

        [JsonProperty("alternates_to_be_removed")]
        public string alternates_to_be_removed { get; set; }
    }

    public class BoxArt
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        //[JsonProperty("gameId")]
        [JsonProperty("games_id")]
        public int GameId { get; set; }

        [JsonProperty("resolution")]
        public string Resolution { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        //**NEW ***********************
        [JsonProperty("userid")]
        public string userid { get; set; }

        [JsonProperty("subkey")]
        public string subkey { get; set; }

        [JsonProperty("username")]
        public string username { get; set; }

        [JsonProperty("dateadded")]
        public string dateadded { get; set; }

        [JsonProperty("languageid")]
        public string languageid { get; set; }

        [JsonProperty("colors")]
        public string colors { get; set; }

        [JsonProperty("artistcolors")]
        public string artistcolors { get; set; }

        [JsonProperty("mirrorupdate")]
        public string mirrorupdate { get; set; }
    }

    public class BaseUrl
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }
    }

    public class UserInfo
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("newguid")]
        public string NewGuid { get; set; }

        [JsonProperty("nextQuery")]
        public string NextQuery { get; set; }
    }
}