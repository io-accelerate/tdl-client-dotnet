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
                var list = jsonNode.ToObject<List<T>>(jsonSerializer);
                if (list == null)
                    throw new InvalidOperationException($"Got null when deserialising to List<{typeof(T).Name}>");
                return list;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize jsonNode to List of {typeof(T).Name}", ex);
            }
        }


        public Dictionary<string, T> GetAsMapOf<T>()
        {
            try
            {
                var list = jsonNode.ToObject<Dictionary<string, T>>(jsonSerializer);
                if (list == null)
                    throw new InvalidOperationException($"Got null when deserialising to Dictionary<string,{typeof(T).Name}>");
                return list;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize jsonNode to Dictionary of {typeof(T).Name}", ex);
            }
        }

        public T GetAsObject<T>()
        {
            try
            {
                var list = jsonNode.ToObject<T>(jsonSerializer);
                if (list == null)
                    throw new InvalidOperationException($"Got null when deserialising to List<{typeof(T).Name}>");
                return list;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize jsonNode to {typeof(T).Name}", ex);
            }
        }
    }
}