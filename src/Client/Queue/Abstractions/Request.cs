using System.Linq;
using Apache.NMS;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TDL.Client.Audit;

namespace TDL.Client.Queue.Abstractions
{
    public class Request : IAuditable
    {
        public ITextMessage? TextMessage { get; set; } // Nullable
        public string? MethodName { get; set; } // Nullable
        public List<JToken>? Params { get; set; } // Nullable
        public string? Id { get; set; } // Nullable

        public string AuditText =>
            $"id = {Id ?? "N/A"}, req = {MethodName ?? "N/A"}({Params?.ToDisplayableRequest() ?? "N/A"})";
    }
}
