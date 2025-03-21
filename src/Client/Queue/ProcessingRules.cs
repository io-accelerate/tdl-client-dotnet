using System;
using System.Collections.Generic;
using TDL.Client.Queue.Abstractions;
using TDL.Client.Queue.Abstractions.Response;
using Newtonsoft.Json.Linq;

namespace TDL.Client.Queue
{
    public partial class ProcessingRules
    {
        private readonly Dictionary<string, ProcessingRule> rules = new();

        public Builder On(string methodName) => new(methodName, this);

        private void Add(
            string methodName,
            Func<List<ParamAccessor>, object> userImplementation)
        {
            rules.Add(methodName, new ProcessingRule(userImplementation));
        }

        public IResponse GetResponseFor(Request request)
        {
            if (!rules.ContainsKey(request.MethodName))
                return new FatalErrorResponse($"method '{request.MethodName}' did not match any processing rule");

            var processingRule = rules[request.MethodName];

            try
            {
                var result = processingRule.UserImplementation(request.Params);
                return new ValidResponse(request.Id, result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new FatalErrorResponse("user implementation raised exception");
            }
        }
    }
}
