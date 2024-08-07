using System;
using System.Net;
using System.Text;
using System.Web;
using RestSharp;

namespace TDL.Client.Runner
{
    public class ChallengeServerClient
    {
        private readonly string journeyId;
        private readonly string acceptHeader;
        private readonly RestClient restClient;

        public ChallengeServerClient(string hostname, int port, string journeyId, bool useColours)
        {
            this.journeyId = journeyId;
            this.acceptHeader = useColours ? "text/coloured" : "text/not-coloured";
            this.restClient = new RestClient(new RestClientOptions($"http://{hostname}:{port}"));
        }

        //~~~~~~~ GET ~~~~~~~~

        public string GetJourneyProgress()
        {
            return Get("journeyProgress");
        }

        public string GetAvailableActions()
        {
            return Get("availableActions");
        }

        public string GetRoundDescription()
        {
            return Get("roundDescription");
        }

        private string Get(string name)
        {
            var encodedPath = HttpUtility.UrlEncode(this.journeyId, Encoding.UTF8);
            var request = new RestRequest($"{name}/{encodedPath}", Method.Get);

            request.AddHeader("Accept", acceptHeader);
            request.AddHeader("Accept-Charset", "UTF-8");

            var response = restClient.Execute(request);
            EnsureStatusOk(response);
            return response.Content;
        }

        //~~~~~~~ POST ~~~~~~~~

        public string SendAction(string name)
        {
            var encodedPath = HttpUtility.UrlEncode(this.journeyId, Encoding.UTF8);
            var request = new RestRequest($"action/{name}/{encodedPath}", Method.Post);

            request.AddHeader("Accept", acceptHeader);
            request.AddHeader("Accept-Charset", "UTF-8");

            var response = restClient.Execute(request);
            EnsureStatusOk(response);
            return response.Content;
        }

        //~~~~~~~ Error handling ~~~~~~~~~

        private static void EnsureStatusOk(RestResponse response)
        {
            if (response.IsSuccessful)
            {
                return;
            }

            var responseStatus = (int)response.StatusCode;
            if (IsClientError(responseStatus))
            {
                throw new ClientErrorException(response.Content);
            }
            else if (IsServerError(responseStatus))
            {
                throw new ServerErrorException(response.StatusDescription);
            }
            else if (IsOtherErrorResponse(responseStatus))
            {
                throw new OtherCommunicationException(response.StatusDescription);
            }
        }

        private static bool IsClientError(int responseStatus)
        {
            return responseStatus >= 400 && responseStatus < 500;
        }

        private static bool IsServerError(int responseStatus)
        {
            return responseStatus >= 500 && responseStatus < 600;
        }

        private static bool IsOtherErrorResponse(int responseStatus)
        {
            return responseStatus < 200 || responseStatus > 299;
        }
    }
}
