using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TDL.Client.Queue.Abstractions;

namespace TDL.Client.Queue.Serialization
{
    public class RequestJson
    {
        [JsonProperty("method")]
        public required string MethodName { get; set; }

        [JsonProperty("params")]
        public required List<JToken> Params { get; set; }

        [JsonProperty("id")]
        public required string Id { get; set; }

        public static RequestJson Deserialize(string value)
        {
            try
            {
                JObject parseResult = JObject.Parse(value);

                // Check if "method" and "id" exist and are not null
                string methodName = (string?)parseResult["method"] ?? throw new DeserializationException("Method name is missing or null.");
                string id = (string?)parseResult["id"]  ?? throw new DeserializationException("ID is missing or null.");

                RequestJson request = new()
                {
                    Id = id,
                    MethodName = methodName,
                    Params = []
                };

                if (parseResult["params"] != null)
                {
                    foreach (JToken param in parseResult["params"]!.Children())
                    {
                        request.Params.Add(param);
                    }
                }
          
                return request;
            }
            catch (JsonReaderException ex)
            {
                throw new DeserializationException("Invalid message format", ex);
            }
        }
    }
}
