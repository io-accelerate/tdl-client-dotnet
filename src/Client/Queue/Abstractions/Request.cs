using System.Linq;
using Apache.NMS;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TDL.Client.Audit;

namespace TDL.Client.Queue.Abstractions
{
    public class Request : IAuditable
    {
        public ITextMessage TextMessage { get; set; }

        public string MethodName { get; set; }

        public List<JToken> Params { get; set; }

        public string Id { get; set; }

        public string AuditText =>
            $"id = {Id}, req = {MethodName}({Params.ToDisplayableRequest()})";
    }
}
