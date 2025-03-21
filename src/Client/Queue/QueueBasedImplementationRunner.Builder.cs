using System;
using TDL.Client.Audit;
using TDL.Client.Queue;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using TDL.Client.Queue.Abstractions;
using Newtonsoft.Json.Serialization;

namespace TDL.Client
{
    public partial class QueueBasedImplementationRunner
    {
        public class Builder
        {
            private readonly ProcessingRules deployProcessingRules;
            private ImplementationRunnerConfig? config;

            private JsonSerializer jsonSerializer;

            public Builder()
            {
                deployProcessingRules = CreateDeployProcessingRules();
                jsonSerializer = new JsonSerializer
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    }
                };
            }

            public Builder SetConfig(ImplementationRunnerConfig config)
            {
                this.config = config;
                return this;
            }

            public Builder WithSolutionFor(string methodName, Func<List<ParamAccessor>, object> userImplementation)
            {
                deployProcessingRules
                    .On(methodName)
                    .Call(userImplementation)
                    .Build();
                return this;
            }

            public QueueBasedImplementationRunner Create()
            {
                if (config is null)
                {
                    throw new InvalidOperationException("Config must be set before creating the runner.");
                }
                return new QueueBasedImplementationRunner(config, deployProcessingRules, jsonSerializer);
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
