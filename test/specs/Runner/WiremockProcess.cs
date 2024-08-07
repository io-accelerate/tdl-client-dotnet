using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using TDL.Test.Specs.Utils;

namespace TDL.Test.Specs.Runner
{
    internal class WiremockProcess
    {
        private readonly RestClient restClient;

        public WiremockProcess(string hostname, int port)
        {
            // Create a new RestClient instance
            restClient = new RestClient(
                new RestClientOptions{ BaseUrl = new Uri($"http://{hostname}:{port}") }, 
                configureSerialization: s => s.UseNewtonsoftJson()
                );

        }

        public void CreateNewMapping(ServerConfig config)
        {
            var request = new RestRequest("__admin/mappings", Method.Post);
            request.AddJsonBody(new WiremockMapping(config));

            restClient.Execute(request);
        }

        public void Reset()
        {
            var request = new RestRequest("__admin/reset", Method.Post);

            restClient.Execute(request);
        }

        public bool EndpointWasHit(string endpoint, string methodType, string body)
        {
            return CountRequestsWithEndpoint(endpoint, methodType, body) == 1;
        }

        private int CountRequestsWithEndpoint(string endpoint, string verb, string body)
        {
            var request = new RestRequest("__admin/requests/count", Method.Post);
            var requestData = new MatchingDataRequest
            {
                Url = endpoint,
                Method = verb
            };

            if (!String.IsNullOrEmpty(body))
            {
                requestData.BodyPatterns = new[]
                {
                    new MatchingDataRequestBodyPattern { EqualTo = body }
                };
            }
            request.AddJsonBody(requestData);

            var response = restClient.Execute<MatchingDataResponse>(request);
            return response.Data?.Count ?? 0;
        }
    }
}
