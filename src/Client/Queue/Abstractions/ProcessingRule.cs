using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TDL.Client.Queue.Abstractions
{
    public class ProcessingRule
    {
        public Func<List<JToken>, object> UserImplementation { get; }

        public ProcessingRule(
            Func<List<JToken>, object> userImplementation)
        {
            UserImplementation = userImplementation;
        }
    }
}
