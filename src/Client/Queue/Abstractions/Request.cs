using System.Linq;
using Apache.NMS;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TDL.Client.Audit;

namespace TDL.Client.Queue.Abstractions
{
    public class Request : IAuditable
    {
        public ITextMessage? TextMessage { get; set; } 
        public required string MethodName { get; set; }
        public required List<JToken> Params { get; set; }
        public required string Id { get; set; }

        public string AuditText =>
            $"id = {Id ?? "N/A"}, req = {MethodName ?? "N/A"}({Params?.ToDisplayableRequest() ?? "N/A"})";
    }
}
