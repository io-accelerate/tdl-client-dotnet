using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TDL.Client.Queue.Abstractions
{
    public class ParamAccessor
    {
        private readonly JToken jsonNode;
        private readonly JsonSerializer jsonSerializer;

        public ParamAccessor(JToken jsonNode, JsonSerializer jsonSerializer)
        {
            this.jsonNode = jsonNode;
            this.jsonSerializer = jsonSerializer;
        }

        public string GetAsString()
        {
            return GetAsObject<string>();
        }

        public int GetAsInteger()
        {
            return GetAsObject<int>();
        }

        public bool IsArray()
        {
            return jsonNode.Type == JTokenType.Array;
        }

        public List<T> GetAsListOf<T>()
        {
            try
            {
                return jsonNode.ToObject<List<T>>(jsonSerializer);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize jsonNode to List of {typeof(T).Name}", ex);
            }
        }

        public T GetAsObject<T>()
        {
            try
            {
                return jsonNode.ToObject<T>(jsonSerializer);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize jsonNode to {typeof(T).Name}", ex);
            }
        }
    }
}