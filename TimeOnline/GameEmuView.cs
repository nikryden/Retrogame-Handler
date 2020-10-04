using RetroGameHandler.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RetroGameHandler.TimeOnline
{
    // Scraper xml Emulationstation
    //  <? xml version="1.0" encoding="UTF-8"?>
    //<gameList>
    //    <game id = "136" source="theGamesDB.net">
    //        <path>./Super Mario World(USA).sfc</path>
    //        <name>Super Mario World(USA)</name>
    //        <desc>Mario&#39;s off on his biggest adventure ever, and this time he&#39;s brought along a friend.  Yoshi the dinosaur teams up with Mario to battle Bowser, who has kidnapped Princess Toadstool once again.  Guide Mario and Yoshi through nine peril-filled worlds to the final showdown in Bowser&#39;s castle.&#xA;&#xA;Use Mario&#39;s new powers and Yoshi&#39;s voracious monster-gobbling appetite as you explore 96 levels filled with dangerous new monsters and traps.  Climb mountains and cross rivers, and descend into subterranean depths.  Destroy the seven Koopa castles and find keys to gain entrance to hidden levels.  Discover more warps and thrilling bonus worlds than ever before!&#xA;&#xA;Mario&#39;s back, and this time he&#39;s better than ever!</desc>
    //        <image>./media/Super Mario World(USA)-image.png</image>
    //        <rating>0.73704</rating>
    //        <releasedate>19901121T000000</releasedate>
    //        <developer>Nintendo</developer>
    //        <publisher>Nintendo</publisher>
    //        <genre>Platform</genre>
    //        <players>2</players>
    //    </game>
    //</gameList>
    [XmlRoot("gameList")]
    public class root
    {
        [XmlElement("game")]
        public List<game> Games { get; set; } = new List<game>();

        public override string ToString()
        {
            using (var stringwriter = new Utf8StringWriter())
            {
                var serializer = new XmlSerializer(this.GetType());
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                serializer.Serialize(stringwriter, this, ns);
                return stringwriter.ToString();
            }
        }

        public static root LoadGamelist(MemoryStream streader)
        {
            streader.Position = 0;
            //StreamReader reader = new StreamReader(streader,Encoding.UTF8);
            //string text = reader.ReadToEnd();
            var serializer = new XmlSerializer(typeof(root));
            return (root)serializer.Deserialize(streader);
        }

        public void save(string FileName)
        {
            //var str = "";
            //using (var stringwriter = new System.IO.StringWriter())
            //{
            //    var serializer = new XmlSerializer(this.GetType(),);
            //    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            //    ns.Add("", "");
            //    serializer.Serialize(stringwriter, this,ns);
            //    str = stringwriter.ToString();
            ////}
            //Console.WriteLine(str);
            var serializer = new XmlSerializer(this.GetType());
            using (var writer = new System.IO.StreamWriter(FileName, false, Encoding.UTF8))
            {
                var sz = new XmlSerializer(this.GetType());
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                sz.Serialize(writer, this, ns);
                writer.Flush();
            }
        }

        public sealed class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }

    public class game
    {
        public game()
        {
        }

        public game(DownloadImageModel item)
        {
            var type = item.GetType();
            var thisType = this.GetType();
            foreach (var t in type.GetProperties())
            {
                var p = thisType.GetProperty(t.Name);
                if (p != null)
                {
                    var value = t.GetValue(item);
                    if (t.Name == "ReleseDate")
                        value = value?.ToString().Replace("-", "") + "T000000" ?? "";
                    p.SetValue(this, value);
                }
            }
        }

        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("source")]
        public string Source { get; } = "theGamesDB.net";

        [XmlElement("name")]
        public string GameTitle { get; set; }

        [XmlElement("releasedate")]
        public string ReleseDate { get; set; }

        [XmlElement("path")]
        public string Path { get; set; }

        [XmlElement("image")]
        public string ImagePath { get; set; }

        [XmlElement("desc")]
        public string Overview { get; set; }

        [XmlElement("players")]
        public string players { get; set; }

        [XmlElement("publisher")]
        public string Publisher { get; set; }

        [XmlElement("rating")]
        public string Rating { get; set; }
    }
}