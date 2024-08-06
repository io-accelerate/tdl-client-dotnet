using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TDL.Client.Queue
{
    public partial class ProcessingRules
    {
        public class Builder
        {
            private readonly ProcessingRules processingRules;
            private readonly string methodName;

            private Func<List<JToken>, object> userImplementation;

            public Builder(string methodName, ProcessingRules processingRules)
            {
                this.methodName = methodName;
                this.processingRules = processingRules;
            }

            public Builder Call(Func<List<JToken>, object> userImplementation)
            {
                this.userImplementation = userImplementation;
                return this;
            }

            public void Build()
            {
                processingRules.Add(methodName, userImplementation);
            }
        }
    }
}
