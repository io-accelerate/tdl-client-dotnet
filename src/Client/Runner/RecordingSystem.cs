using System;
using System.Net;
using RestSharp;
using TDL.Client.Runner.Exceptions;

namespace TDL.Client.Runner
{
    public class RecordingEvent 
    {
        public static readonly string ROUND_START = "new";
        public static readonly string ROUND_SOLUTION_DEPLOY = "deploy";
        public static readonly string ROUND_COMPLETED = "done";
    }

    public class RecordingSystem : IRoundChangesListener
    {
        private const string RecordingSystemEndpoint = "http://localhost:41375";

        private static readonly RestClient RestClient = new(new RestClientOptions(RecordingSystemEndpoint));

        private bool recordingRequired;

        public RecordingSystem(bool recordingRequired)
        {
            this.recordingRequired = recordingRequired;
        }

        public bool IsRecordingSystemOk()
        {
            return recordingRequired
                ? IsRunning()
                : true;
        }

        public static bool IsRunning()
        {
            try
            {
                var request = new RestRequest("status", Method.Get);
                var response = RestClient.Execute(request);

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                throw new RecordingSystemNotReachable(e);
            }
        }

        public void NotifyEvent(string lastFetchedRound, string eventName)
        {
            Console.WriteLine($@"Notify round ""{lastFetchedRound}"", event ""{eventName}""");
            SendPost("/notify", $"{lastFetchedRound}/{eventName}");
        }

        public void TellToStop()
        {
            Console.WriteLine("Stopping recording system");
            SendPost("/stop", string.Empty);
        }

        private void SendPost(string endpoint, string body)
        {
            if (!recordingRequired)
            {
                return;
            }

            try
            {
                var request = new RestRequest(endpoint, Method.Post);
                request.AddBody(body); // Updated method for adding body content
                var response = RestClient.Execute(request);
                var responseContent = response.Content ?? "";

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine($"Recording system returned code: {response.StatusCode}");
                }
                else if (!responseContent.StartsWith("ACK", StringComparison.Ordinal))
                {
                    Console.WriteLine($"Recording system returned body: {responseContent}");
                }
            }
            catch (Exception e)
            {
                throw new RecordingSystemNotReachable(e);
            }
        }

        public void OnNewRound(string roundId)
        {
            NotifyEvent(roundId, RecordingEvent.ROUND_START);
        }
    }
}
