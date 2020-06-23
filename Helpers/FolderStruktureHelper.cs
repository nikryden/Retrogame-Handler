using RetroGameHandler.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RetroGameHandler.Helpers
{
    internal class FolderStruktureHelper
    {
        public Dictionary<string, List<string>> BuildFolderStructure()
        {
            var list = TheGamesDbHandler.PlatformModels;
            var dictionary = new Dictionary<string, List<string>>();
            foreach (var itm in list)
            {
                if (dictionary.ContainsKey(itm.Developer))
                {
                    dictionary[itm.Developer].Add(itm.Name);
                }
                else
                {
                    dictionary.Add(itm.Developer, new List<string>() { itm.Name });
                }
            }
            return dictionary;
        }

        private string shortName(string name)
        {
            var dictionaryTranslate = new Dictionary<string, string>() {
                        {"Nintendo 64","N64" },
                        {"Nintendo DS","DS" },
                        {"Nintendo Entertainment System (NES)","NES" },
                        {"Nintendo Game Boy","GB" },
                        {"Nintendo Game Boy Advance","GBA" },
                        {"Nintendo Game Boy Color","GBC" },
                        {"Atari 2600","2600" },
                        {"Atari 5200","5200" },
                        {"Atari 7800","7800" },
                        {"Atari 800","800" },
                        {"Atari Jaguar","Jaguar" },
                        {"Atari ST","ST" },
                        {"Atari XE","XE" },
                        {"Sega 32X","32X" },
                        {"Sega CD","CD" },
                        {"Sega Game Gear","GameGear" },
                        {"Sega Genesis","Genesis" },
                        {"Sega Master System","MS" },
                        {"Sega Mega Drive","MDrive" },
                        {"Sega Pico","Pico" },
                        {"Sega Saturn","Saturn" },
                        {"SEGA SG-1000","SG-1000" },
                        {"Sony Playstation","Playstation" },
                        {"Sony Playstation Portable","psp" }
                    };

            return dictionaryTranslate.ContainsKey(name) ? dictionaryTranslate[name] : null; ;
        }
    }
}