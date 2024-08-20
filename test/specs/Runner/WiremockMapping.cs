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
                UrlPattern = config.EndpointMatches!,
                Url = config.EndpointEquals!,
                Method = config.Verb!
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
                Body = config.ResponseBody!,
                StatusMessage = config.StatusMessage!,
                Status = config.Status
            };
        }
    }

    internal class WiremockMappingResponse
    {
        [JsonProperty("body")]
        public required string Body { get; set; }

        [JsonProperty("statusMessage")]
        public required string StatusMessage { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }

    internal class WiremockMappingRequest
    {
        [JsonProperty("urlPattern")]
        public required string UrlPattern { get; set; }

        [JsonProperty("url")]
        public required string Url { get; set; }

        [JsonProperty("method")]
        public required string Method { get; set; }

        [JsonProperty("headers")]
        public WiremockMappingHeader? Headers { get; set; }
    }

    internal class WiremockMappingHeader
    {
        [JsonProperty("Accept")]
        public required WiremockMappingHeaderAccept Accept { get; set; }
    }

    internal class WiremockMappingHeaderAccept
    {
        [JsonProperty("contains")]
        public required string Contains { get; set; }
    }

    internal class MatchingDataRequest
    {
        [JsonProperty("urlPattern")]
        public string? UrlPattern { get; set; }

        [JsonProperty("url")]
        public required string Url { get; set; }

        [JsonProperty("method")]
        public required string Method { get; set; }

        [JsonProperty("bodyPatterns")]
        public MatchingDataRequestBodyPattern[]? BodyPatterns { get; set; }
    }

    internal class MatchingDataRequestBodyPattern
    {
        [JsonProperty("equalTo")]
        public required string EqualTo { get; set; }
    }

    internal class MatchingDataResponse
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
