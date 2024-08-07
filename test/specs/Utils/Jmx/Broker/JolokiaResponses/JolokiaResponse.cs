using System.Net;
using Newtonsoft.Json;

namespace TDL.Test.Specs.Utils.Jmx.Broker.JolokiaResponses
{
    internal class JolokiaResponse
    {
        [JsonProperty("status")]
        public HttpStatusCode Status { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("error_type")]
        public string? ErrorType { get; set; }

        [JsonProperty("error")]
        public string? Error { get; set; }

        [JsonProperty("stacktrace")]
        public string? Stacktrace { get; set; }
    }

    internal class JolokiaResponse<T> : JolokiaResponse
    {
        [JsonProperty("value")]
        public T? Value { get; set; }
    }
}
