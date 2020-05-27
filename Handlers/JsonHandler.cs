using System;
using System.Net;
using Newtonsoft.Json;

namespace RetroGameHandler.Handlers
{
    public class JsonHandler
    {
        public static T DownloadSerializedJsonData<T>(string url) where T : new()
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex);
                    return new T();
                }
                // if string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
            }
        }
    }
}
