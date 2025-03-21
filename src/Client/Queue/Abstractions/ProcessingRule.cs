using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TDL.Client.Queue.Abstractions
{
    public class ProcessingRule
    {
        public Func<List<ParamAccessor>, object> UserImplementation { get; }

        public ProcessingRule(
            Func<List<ParamAccessor>, object> userImplementation)
        {
            UserImplementation = userImplementation;
        }
    }
}
