using System;
using TDL.Client.Audit;
using TDL.Client.Queue;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TDL.Client
{
    public partial class QueueBasedImplementationRunner
    {
        public class Builder
        {
            private ProcessingRules deployProcessingRules;
            private ImplementationRunnerConfig? config; // Make config nullable

            public Builder()
            {
                deployProcessingRules = CreateDeployProcessingRules();
            }

            public Builder SetConfig(ImplementationRunnerConfig config)
            {
                this.config = config;
                return this;
            }

            public Builder WithSolutionFor(string methodName, Func<List<JToken>, object> userImplementation)
            {
                deployProcessingRules
                    .On(methodName)
                    .Call(userImplementation)
                    .Build();
                return this;
            }

            public QueueBasedImplementationRunner Create()
            {
                if (config == null)
                {
                    throw new InvalidOperationException("Config must be set before creating the runner.");
                }
                return new QueueBasedImplementationRunner(config, deployProcessingRules);
            }

            private static ProcessingRules CreateDeployProcessingRules()
            {
                var deployProcessingRules = new ProcessingRules();

                // Debt - we only need this to consume message from the server
                deployProcessingRules
                        .On("display_description")
                        .Call(p => "OK")
                        .Build();

                return deployProcessingRules;
            }
        }
    }
}
