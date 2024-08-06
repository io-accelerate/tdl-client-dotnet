using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TDL.Client.Queue.Abstractions;

namespace TDL.Client.Queue.Serialization
{
    public class RequestJson
    {
        [JsonProperty("method")]
        public string MethodName { get; set; }

        [JsonProperty("params")]
        public List<JToken> Params { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        public Request To() =>
            new Request
            {
                MethodName = MethodName,
                Params = Params,
                Id = Id
            };

        public static RequestJson Deserialize(string value)
        {
            try
            {
                JObject parseResult = JObject.Parse(value);
                RequestJson request = new RequestJson();
                request.MethodName = (string)parseResult["method"];
                request.Params = new List<JToken>();
                foreach (JToken param in parseResult["params"].Children())
                {
                    request.Params.Add(param);
                }
                request.Id = (string)parseResult["id"];
                return request;
            }
            catch (JsonReaderException ex)
            {
                throw new DeserializationException("Invalid message format", ex);
            }
        }
    }
}
