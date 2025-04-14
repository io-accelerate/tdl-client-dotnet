using System;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TDL.Client.Queue.Abstractions;
using Newtonsoft.Json;

namespace TDL.Test.Specs.Queue.Factories
{
    internal static class CallImplementationFactory
    {

        private class TestItem
        {
            public string Field1 { get; }
            
            public int? Field2 { get; }

            public TestItem(string field1, int? field2)
            {
                Field1 = field1;
                Field2 = field2;
            }
        }

        private static readonly Dictionary<string, Func<List<ParamAccessor>, object>> CallImplementations =
            new()
            {
                ["add two numbers"] = args =>
                    args[0].GetAsInteger() + args[1].GetAsInteger(),

                ["increment number"] = args =>
                    args[0].GetAsInteger() + 1,

                ["return null"] = args =>
                    null!,

                ["throw exception"] = args =>
                    throw new InvalidOperationException("faulty user code"),

                ["replay the value"] = args =>
                    args[0].GetAsString()!,

                ["sum the elements of an array"] = args =>
                {
                    var numbers = args[0].GetAsListOf<int>();
                    return numbers.Sum();
                },

                ["generate array of integers"] = args =>
                {
                    int startIncl = args[0].GetAsInteger();
                    int endExcl = args[1].GetAsInteger();
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
                },

                ["concatenate fields as string"] = args =>
                {
                    var testItem = args[0].GetAsObject<TestItem>();
                    return testItem.Field1 + testItem.Field2;
                },

                ["build an object with two fields"] = args =>
                {
                    var field1 = args[0].GetAsString();
                    var field2 = args[1].GetAsInteger();
                    return new TestItem(field1, field2);
                },

                ["retrieve a value from a map"] = args =>
                {
                    return args[0].GetAsMapOf<string>().GetValueOrDefault("key1", "not found");
                }
            };


        public static Func<List<ParamAccessor>, object> Get(string call)
        {
            if (!CallImplementations.ContainsKey(call))
            {
                throw new ArgumentException($@"Not a valid implementation reference: ""{call}"");");
            }

            return CallImplementations[call];
        }
    }
}
