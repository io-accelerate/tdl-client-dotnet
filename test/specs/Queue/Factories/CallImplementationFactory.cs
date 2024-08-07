using System;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TDL.Test.Specs.Queue.Factories
{
    internal static class CallImplementationFactory
    {
        private static readonly Dictionary<string, Func<List<JToken>, object>> CallImplementations =
            new()
            {
                ["add two numbers"] = args =>
                    (int)args[0] + (int)args[1],
                ["increment number"] = args =>
                    (int)args[0] + 1,
                ["return null"] = args =>
                    null,
                ["throw exception"] = args =>
                    throw new InvalidOperationException("faulty user code"),
                ["replay the value"] = args =>
                    (string)args[0],
                ["sum the elements of an array"] = args =>
                {
                    JArray intArray = (JArray)args[0];
                    return intArray.Select(c => (int)c).Sum();
                },
                ["generate array of integers"] = args =>
                {
                    int startIncl = (int)args[0];
                    int endExcl = (int)args[1];
                    return Enumerable.Range(startIncl, endExcl - startIncl).ToList();
                },
                ["some logic"] = args =>
                    "ok",
                ["work for 600ms"] = args =>
                {
                    try
                    {
                        Thread.Sleep(600);
                    }
                    catch (ThreadInterruptedException ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    return "OK";
                }
            };

        public static Func<List<JToken>, object> Get(string call)
        {
            if (!CallImplementations.ContainsKey(call))
            {
                throw new ArgumentException($@"Not a valid implementation reference: ""{call}"");");
            }

            return CallImplementations[call];
        }
    }
}
