using Newtonsoft.Json;

namespace TDL.Test.Specs.Runner
{
    internal class WiremockMapping
    {
        [JsonProperty("request")]
        public WiremockMappingRequest Request { get; set; }

        [JsonProperty("response")]
        public WiremockMappingResponse Response { get; set; }

        public WiremockMapping(ServerConfig config)
        {
            Request = new WiremockMappingRequest
            {
                UrlPattern = config.EndpointMatches,
                Url = config.EndpointEquals,
                Method = config.Verb
            };

            if (config.AcceptHeader != null)
            {
                Request.Headers = new WiremockMappingHeader
                {
                    Accept = new WiremockMappingHeaderAccept
                    {
                        Contains = config.AcceptHeader
                    }
                };
            }

            Response = new WiremockMappingResponse
            {
                Body = config.ResponseBody,
                StatusMessage = config.StatusMessage,
                Status = config.Status
            };
        }
    }

    internal class WiremockMappingResponse
    {
        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("statusMessage")]
        public string StatusMessage { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }

    internal class WiremockMappingRequest
    {
        [JsonProperty("urlPattern")]
        public string UrlPattern { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("headers")]
        public WiremockMappingHeader Headers { get; set; }
    }

    internal class WiremockMappingHeader
    {
        [JsonProperty("Accept")]
        public WiremockMappingHeaderAccept Accept { get; set; }
    }

    internal class WiremockMappingHeaderAccept
    {
        [JsonProperty("contains")]
        public string Contains { get; set; }
    }

    internal class MatchingDataRequest
    {
        [JsonProperty("urlPattern")]
        public string UrlPattern { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("bodyPatterns")]
        public MatchingDataRequestBodyPattern[] BodyPatterns { get; set; }
    }

    internal class MatchingDataRequestBodyPattern
    {
        [JsonProperty("equalTo")]
        public string EqualTo { get; set; }
    }

    internal class MatchingDataResponse
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
