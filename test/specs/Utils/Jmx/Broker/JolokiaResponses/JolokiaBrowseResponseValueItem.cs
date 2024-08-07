using Newtonsoft.Json;

namespace TDL.Test.Specs.Utils.Jmx.Broker.JolokiaResponses
{
    internal class JolokiaBrowseResponseValueItem
    {
        [JsonProperty("Text")]
        public string? Text { get; set; }
    }
}
