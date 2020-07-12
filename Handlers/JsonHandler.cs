using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Windows.Media.Animation;
using Newtonsoft.Json;

namespace RetroGameHandler.Handlers
{
    public class JsonHandler
    {
        public static T DownloadSerializedJsonData<T>(string url, Dictionary<string, string> headers = null, int retry = 0) where T : new()
        {
            using (var w = new WebClient())
            {
                //if (token != null) w.Headers.Add("token", token);
                if (headers != null) foreach (var item in headers)
                    {
                        w.Headers.Add(item.Key, item.Value);
                    }
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (WebException ex)
                {
                    if (retry <= 3)
                    {
                        ErrorHandler.Debug(ex.ToString());
                        return DownloadSerializedJsonData<T>(url, headers, retry + 1);
                    }
                    else
                    {
                        using (StreamReader r = new StreamReader(ex.Response.GetResponseStream()))
                        {
                            json_data = r.ReadToEnd();
                            ErrorHandler.Error(json_data, Entities.ErrorLevel.ERROR, ex.ToString(), ex.StackTrace);
                        }
                        System.Windows.MessageBox.Show(ex.Message, "Error!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        //return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
                        return new T();
                    }

                    //return new T();
                }
                // if string with JSON data is not empty, deserialize it to class and return its instance
                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
            }
        }
    }
}