using RetroGameHandler.Entities;
using RetroGameHandler.models;
using RetroGameHandler.TimeOnline;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RetroGameHandler.Handlers
{
    public class TheGamesDbHandler
    {
        private static IEnumerable<PlatformEntity> platforms;
        public static ObservableCollection<PlatformModel> PlatformModels = new ObservableCollection<PlatformModel>();

        private static string PlatformImagePath { get; } = "https://cdn.thegamesdb.net/images/original/consoles/png48/";

        private static string ApiKey = "946f7777fa10bc47c4b5992ec16d646b1d4d7c0c7b9beb778d88534dd4c75278";

        //private static string PlatformApiPath { get; } = "https://api.thegamesdb.net/v1/Platforms?apikey=946f7777fa10bc47c4b5992ec16d646b1d4d7c0c7b9beb778d88534dd4c75278&fields=icon%2Cconsole%2Ccontroller%2Cdeveloper%2Cmanufacturer%2Cmedia%2Ccpu%2Cmemory%2Cgraphics%2Csound%2Cmaxcontrollers%2Cdisplay%2Coverview%2Cyoutube";
        private static string PlatformApiPath { get; } = "https://timeonline.se/api/platforms";

        private static string Mac = GetMACAddress();
        public static List<BaseUrl> BaseUrls;

        public async static Task<GameRoot> GetGame(string gameName, int platformId, bool useDirectory, string token)
        {
            try
            {
                gameName = CleanGameName(gameName, useDirectory);
                if (string.IsNullOrWhiteSpace(gameName)) return new GameRoot();
                var path = RGHSettings.ScrapPath + $@"games/{platformId}/{gameName}";
                var headers = new Dictionary<string, string>();

                headers.Add("token", token);
                headers.Add("secret", RGHSettings.ProgGuid);

                headers.Add("version", RGHSettings.Version);
                return await Task.Run(() => JsonHandler.DownloadSerializedJsonData<GameRoot>(path, headers));
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                throw;
            }
        }

        public static string GetMACAddress()
        {
            try
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                string MACAddress = String.Empty;
                foreach (ManagementObject mo in moc)
                {
                    if (MACAddress == String.Empty)
                    {
                        if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                    }
                    mo.Dispose();
                }

                MACAddress = MACAddress.Replace(":", "");
                return MACAddress;
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                throw;
            }
        }

        //public static string GetMACAddress()
        //{
        //    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration where IPEnabled=true");
        //    IEnumerable<ManagementObject> objects = searcher.Get().Cast<ManagementObject>();
        //    string mac = (from o in objects orderby o["IPConnectionMetric"] select o["MACAddress"].ToString()).FirstOrDefault();
        //    return mac;
        //}

        public static string ReplaceIntToRomanNumberInText(string value)
        {
            string fixThis = value;
            var re = new Regex("\\d+");

            string result = "";
            int lastIndex = 0;
            string lastMatch = "";
            //Get the first match using the regular expression:
            var m = re.Match(fixThis);

            //Keep looping while we can match:
            while (m.Success)
            {
                //Get length of text between last match and current match:
                int len = m.Index - (lastIndex + lastMatch.Length);
                result += fixThis.Substring(lastIndex + lastMatch.Length, len) + GetRomanText(m);

                //Save values for next iteration:
                lastIndex = m.Index;
                lastMatch = m.Value;
                m = m.NextMatch();
            }

            //Append text after last match:
            if (lastIndex > 0)
            {
                result += fixThis.Substring(lastIndex + lastMatch.Length);
            }
            return result;
        }

        private static string GetRomanText(Match m)
        {
            string result = "";
            // Get ASCII value of first digit from the match (remember, 48= ascii 0, 57=ascii 9):
            char c = m.Value[0];
            if (c >= 48 && c <= 57)
            {
                int index = c - 48;
                result = ToRoman(index);
            }

            return result;
        }

        public static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }

        public static string CleanGameName(string gameName, bool useDirectory)
        {
            try
            {
                gameName = Regex.Replace(gameName, @"\([^()]*\)", string.Empty);
                gameName = Regex.Replace(gameName, @"\[[^()]*\]", string.Empty);
                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                gameName = regex.Replace(gameName, " ");
                //gameName = Regex.Replace(gameName, "[ ]{ 2,}", " ");
                //gameName = Regex.Replace(gameName, " {2,}", "%");
                gameName = AddSpacesBeforeUpperCase(gameName);
                //gameName = gameName.Replace("&", "%26");
                //gameName = gameName.Replace("&", "&amp;");
                gameName = gameName.Replace("&", "%");
                gameName = gameName.Replace("\"", "%");
                gameName = gameName.Replace(" The ", "%");
                //gameName = gameName.Replace(" - ", "%");
                // gameName = gameName.Replace("-", "%");
                //gameName = gameName.Replace(", ", "%");
                //gameName = gameName.Replace(",", "%");

                if (!useDirectory) gameName = System.IO.Path.GetFileNameWithoutExtension(gameName).Trim();
                return gameName;
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                throw;
            }
        }

        public static string AddSpacesBeforeUpperCase(string nonSpacedString)
        {
            if (string.IsNullOrEmpty(nonSpacedString))
                return string.Empty;

            StringBuilder newText = new StringBuilder(nonSpacedString.Length * 2);
            newText.Append(nonSpacedString[0]);

            for (int i = 1; i < nonSpacedString.Length; i++)
            {
                char currentChar = nonSpacedString[i];
                // If it is whitespace, we do not need to add another next to it
                if (char.IsWhiteSpace(currentChar))
                {
                    newText.Append(' ');
                    continue;
                }

                char previousChar = nonSpacedString[i - 1];

                char nextChar = i < nonSpacedString.Length - 1 ? nonSpacedString[i + 1] : nonSpacedString[i];
                if (char.IsWhiteSpace(previousChar))
                {
                    newText.Append(currentChar);
                    continue;
                }
                if (!char.IsLetter(currentChar) || char.IsWhiteSpace(currentChar))
                {
                    newText.Append(' ');
                }
                else if (char.IsUpper(currentChar) && !char.IsWhiteSpace(nextChar)
                  && !(char.IsUpper(previousChar) && char.IsUpper(nextChar)))
                {
                    newText.Append(' ');
                }
                else if (i < nonSpacedString.Length)
                {
                    if (char.IsUpper(currentChar) && !char.IsWhiteSpace(nextChar) && !char.IsUpper(nextChar))
                    {
                        newText.Append(' ');
                    }
                }

                newText.Append(currentChar);
            }

            return newText.ToString();
        }

        public async static Task<List<string>> GetGameOkExtensions(int id)
        {
            var gameExts = await Task.Run(() => JsonHandler.DownloadSerializedJsonData<GameExtensions>(""));
            return gameExts.Emulators.FirstOrDefault(emu => emu.Id == id)?.Extensions;
        }

        public async static void init()
        {
            try
            {
                Mac = GetMACAddress();

                #region Platforms

                var dataEntity = LiteDBHelper.Load<DataModel>().FirstOrDefault() ?? new DataModel() { Id = 1, ScrapGuid = "", ScrapEmail = "" };
                //var burl = await GetGame("¤", 1, false, dataEntity.ScrapGuid);
                //BaseUrls = burl.BaseUrls;

                var tmp = await LiteDBHelper.LoadAsync<PlatformResponse>();
                //platforms = tmp.FirstOrDefault()?.data.Platforms.Select(p => p.Value) ?? null;
                platforms = null;
                Dictionary<int, List<string>> extensions;
                extensions = await Task.Run(() => JsonHandler.DownloadSerializedJsonData<EmuExtensionsEntity>("http://timeonline.se/RGHandler/EmulatorSupportList.json").Extensions.ToDictionary(em => em.id, em => em.extensoins));
                if (platforms == null)
                {
                    PlatformResponse p = null;
                    var headers = new Dictionary<string, string>();

                    headers.Add("secret", RGHSettings.ProgGuid);
                    headers.Add("version", RGHSettings.Version);
                    await Task.Run(() => p = JsonHandler.DownloadSerializedJsonData<PlatformResponse>(PlatformApiPath, headers));

                    if (p != null)
                    {
                        await Task.Run(() =>
                        {
                            platforms = p.data?.Platforms;/*?.Select(i => i.Value);*/

                            foreach (var pl in platforms)
                            {
                                pl.Extensions = extensions.ContainsKey(pl.Id) && !extensions[pl.Id].Any(s => s == "?") ? extensions[pl.Id] : new List<string>();
                                var icon = pl.Icon;
                                var url = PlatformImagePath + icon;
                                using (WebClient client = new WebClient())
                                {
                                    byte[] data = client.DownloadData(url);
                                    using (MemoryStream mem = new MemoryStream(data))
                                    {
                                        LiteDBHelper.SaveFile("platform/images", icon, mem);
                                    }
                                }
                            }
                        });
                        LiteDBHelper.Save(p);
                    }
                }
                else
                {
                    foreach (var pl in platforms) pl.Extensions = extensions.ContainsKey(pl.Id) && !extensions[pl.Id].Any(s => s == "?") ? extensions[pl.Id] : new List<string>();
                }

                foreach (var pl in platforms)
                {
                    using (MemoryStream mem = new MemoryStream())
                    {
                        LiteDBHelper.LoadFile("platform/images", pl.Icon, mem);
                        PlatformModels.Add(new PlatformModel(pl, LoadImage(mem)));
                    }
                }

                #endregion Platforms
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                throw;
            }
        }

        public static BitmapImage LoadImage(Stream stream)
        {
            try
            {
                stream.Position = 0;
                // assumes that the streams position is at the beginning
                // for example if you use a memory stream you might need to point it to 0 first
                var image = new BitmapImage();

                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();

                image.Freeze();
                return image;
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                throw;
            }
        }

        public static void FindGame(string searchString, int platformId)
        {
        }
    }
}