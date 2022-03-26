using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using RestSharp;
using RetroGameHandler.thegamesAPI;
using RetroGameHandler.thegamesAPI.Games;
using RetroGameHandler.thegamesAPI.Image;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RetroGameHandler.Handlers
{
    public class RestHandler : INotifyPropertyChanged
    {
        public string APIKey { get; } = "1243e5c43c3c9070009e8ad7a2adebcf0bce6ef344de55ba307bd31271c7f628";

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task<Games> GetGameByName(string name)
        {
            try
            {
                var client = new RestClient("https://api.thegamesdb.net/v1.1/Games/ByGameName");
                RestRequest request = new RestRequest("",Method.Get);
                request.AddParameter("apikey", APIKey);
                request.AddParameter("name", name);
                request.AddParameter("include", "boxart,platform");
                request.AddHeader("Content-Type", "application/json");
                var response = await client.ExecuteAsync(request);
                return JsonConvert.DeserializeObject<Games>(response.Content);
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                throw;
            }
        }

        public async Task<Images> GetGameImage(long id)
        {
            try
            {
                var client = new RestClient("https://api.thegamesdb.net/v1/Games/Images");
                RestRequest request = new RestRequest("",Method.Get);
                request.AddParameter("apikey", APIKey);
                request.AddParameter("games_id", id);

                request.AddHeader("Content-Type", "application/json");
                var response = await client.ExecuteAsync(request);
                return JsonConvert.DeserializeObject<Images>(response.Content);
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                throw;
            }
        }

        public async Task GetConsoleList()
        {
            var client = new RestClient("https://api.thegamesdb.net/v1/Platforms");
            RestRequest request = new RestRequest("", Method.Get);
            //https://api.thegamesdb.net/v1.1/Games/ByGameName?apikey=1243e5c43c3c9070009e8ad7a2adebcf0bce6ef344de55ba307bd31271c7f628&name=3%20Ninjas%20Kick%20Back
            //https://api.thegamesdb.net/v1/Platforms?apikey=1243e5c43c3c9070009e8ad7a2adebcf0bce6ef344de55ba307bd31271c7f628
            //https://api.thegamesdb.net/v1/Games/Images?apikey=1243e5c43c3c9070009e8ad7a2adebcf0bce6ef344de55ba307bd31271c7f628&games_id=495
            request.AddParameter("apikey", "946f7777fa10bc47c4b5992ec16d646b1d4d7c0c7b9beb778d88534dd4c75278");
            //request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,

                Converters = new List<JsonConverter>
                {
                    new IsoDateTimeConverter()
                    {
                        DateTimeFormat= "yyyy-MM-dd HH:mm:ss"
                    }
                }
            };

            var response = await client.ExecuteAsync(request);
            rootObj obj = JsonConvert.DeserializeObject<rootObj>(response.Content);
            //To make the call async
            //var cancellationTokenSource = new CancellationTokenSource();
            //var response2 = restClient.ExecuteTaskAsync<T>(request, cancellationTokenSource.Token);

            if (response.ErrorException != null)
            {
                ErrorHandler.Error(response.ErrorException);
                const string message = "Error retrieving response.  Check inner details for more info.";
                var browserStackException = new ApplicationException(message, response.ErrorException);
                throw browserStackException;
            }
            //return response.Content;
        }
    }

    public class MyModelConverter : JsonConverter
    {
        //objectType is the type as specified for List<myModel> (i.e. myModel)
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                var token = JToken.Load(reader); //json from myModelList > model
                var list = Activator.CreateInstance(objectType) as System.Collections.IList; // new list to return
                var itemType = objectType.GenericTypeArguments[0]; // type of the list (myModel)
                if (token.Type.ToString() == "Object") //Object
                {
                    var child = token.Children();
                    var n = child.AsJEnumerable();
                    foreach (var s in n)
                    {
                        var x = s.Children();
                        foreach (var t in x)
                        {
                            var newObject = Activator.CreateInstance(itemType);
                            if (t.Type == JTokenType.Array)
                            {
                                foreach (var c in t.Children())
                                {
                                    serializer.Populate(c.CreateReader(), newObject);
                                    list.Add(newObject);
                                }
                            }
                            else
                            {
                                serializer.Populate(t.CreateReader(), newObject);
                                list.Add(newObject);
                            }
                        }
                    }
                }
                else //Array
                {
                    foreach (var child in token.Children())
                    {
                        var newObject = Activator.CreateInstance(itemType);
                        serializer.Populate(child.CreateReader(), newObject);
                        list.Add(newObject);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                throw;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && (objectType.GetGenericTypeDefinition() == typeof(List<>));
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}