using RestSharp.Serializers;

namespace TDL.Test.Specs.Runner
{
    internal class ServerConfig
    {
        public string Verb { get; set; }
        public string EndpointEquals { get; set; }
        public string EndpointMatches { get; set; }
        public int Status { get; set; }
        public string ResponseBody { get; set; }
        public string AcceptHeader { get; set; }
        public string StatusMessage { get; set; }
    }
}
