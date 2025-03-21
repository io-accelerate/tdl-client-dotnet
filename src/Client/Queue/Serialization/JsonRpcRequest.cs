using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TDL.Client.Queue.Abstractions;

namespace TDL.Client.Queue.Serialization
{
    public class JsonRpcRequest
    {
        [JsonProperty("method")]
        public required string MethodName { get; set; }

        [JsonProperty("params")]
        public required List<JToken> Params { get; set; }

        [JsonProperty("id")]
        public required string Id { get; set; }
    }
}
