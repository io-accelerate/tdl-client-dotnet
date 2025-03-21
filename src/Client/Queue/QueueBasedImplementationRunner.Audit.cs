using System;
using System.Text;
using Newtonsoft.Json;
using TDL.Client.Audit;
using TDL.Client.Queue.Abstractions;
using TDL.Client.Queue.Abstractions.Response;

namespace TDL.Client
{
    public partial class QueueBasedImplementationRunner
    {
        private class Audit
        {
            private readonly IAuditStream auditStream;
            private readonly List<string> messages = new();
            private readonly PresentationUtils presentationUtils;

            public Audit(IAuditStream auditStream, JsonSerializer jsonSerializer)
            {
                this.auditStream = auditStream;
                this.presentationUtils = new PresentationUtils(jsonSerializer);
            }

            //~~~ Normal output

            public void StartLine()
            {
                messages.Clear();
            }

            public void LogRequest(Request request)
            {
                string requestLogLine = $"id = {request.Id}, req = {request.MethodName}({presentationUtils.ToDisplayableRequest(request.Params)})";
                messages.Add(requestLogLine);
            }

            public void LogResponse(IResponse response)
            {
                string responseLogLine;
                if (response.IsError)
                {
                    responseLogLine = $"{response.Id} = \"{response.Result}\", (NOT PUBLISHED)";
                }
                else
                {
                    responseLogLine = $"resp = {presentationUtils.ToDisplayableResponse(response.Result)}";
                }
                messages.Add(responseLogLine);
            }

            public void EndLine()
            {
                auditStream.WriteLine(string.Join(", ", messages));
            }

            //~~~ Exception

            public void LogException(string message, Exception e)
            {
                StartLine();
                messages.Add($"{message}: {e.Message}");
                EndLine();
            }

            public void LogLine(string text)
            {
                StartLine();
                messages.Add(text);
                EndLine();
            }
        }

    }
}
