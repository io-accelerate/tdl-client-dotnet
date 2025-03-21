using Newtonsoft.Json;
using TDL.Client.Queue.Abstractions.Response;

namespace TDL.Client.Queue.Serialization
{
    public class JsonRpcResponse
    {
        [JsonProperty("result")]
        public required object Result { get; set; }

        [JsonProperty("error")]
        public string? Error { get; set; }

        [JsonProperty("id")]
        public required string Id { get; set; }
    }
}
