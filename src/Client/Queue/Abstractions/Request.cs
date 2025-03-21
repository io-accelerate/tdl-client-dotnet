using System.Linq;
using Apache.NMS;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TDL.Client.Audit;

namespace TDL.Client.Queue.Abstractions
{
    public class Request
    {
        public ITextMessage? TextMessage { get; set; } 
        public required string MethodName { get; set; }
        public required List<ParamAccessor> Params { get; set; }
        public required string Id { get; set; }
    }
}
