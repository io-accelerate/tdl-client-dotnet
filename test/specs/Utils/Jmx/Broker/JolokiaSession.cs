using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using TDL.Test.Specs.Utils.Jmx.Broker.JolokiaResponses;

namespace TDL.Test.Specs.Utils.Jmx.Broker
{
    internal class JolokiaSession
    {
        private readonly RestClient restClient;

        private JolokiaSession(Uri jolokiaUri)
        {
            restClient = new RestClient(jolokiaUri);
            restClient = new RestClient(
                new RestClientOptions{ BaseUrl = jolokiaUri }, 
                configureSerialization: s => s.UseNewtonsoftJson()
                );
        }

        public static JolokiaSession Connect(string host, int adminPort)
        {
            var jolokiaUri = new Uri($"http://{host}:{adminPort}/api/jolokia");

            return new JolokiaSession(jolokiaUri);
        }

        public JolokiaResponse<string> Request(Dictionary<string, object> jolokiaPayload)
        {
            return Request<string>(jolokiaPayload);
        }

        public JolokiaResponse<T> Request<T>(Dictionary<string, object> jolokiaPayload)
        {
            // Use RestRequest constructor with HttpMethod.Post for POST requests
            var request = new RestRequest("", Method.Post)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Origin", "http://localhost");
            request.AddJsonBody(jolokiaPayload);

            var response = restClient.Execute<JolokiaResponse<T>>(request);

            ValidateResponse(response.StatusCode, response.Content);

            // Make sure response.Data is not null before accessing its properties
            if (response.Data != null)
            {
                ValidateResponse(response.Data.Status, $"{response.Data.ErrorType}: {response.Data.Error}");
            }

            return response.Data;
        }

        private static void ValidateResponse(HttpStatusCode responseStatusCode, string content)
        {
            if (responseStatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Failed Jolokia call: {responseStatusCode}: {content}");
            }
        }
    }
}
