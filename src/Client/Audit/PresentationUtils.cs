using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TDL.Client.Queue.Abstractions;

namespace TDL.Client.Audit
{
    public sealed class PresentationUtils
    {
        private readonly JsonSerializer jsonSerializer;

        public PresentationUtils(JsonSerializer jsonSerializer)
        {
            this.jsonSerializer = jsonSerializer;
        }

        public string ToDisplayableRequest(List<ParamAccessor> items)
        {
            var sb = new StringBuilder();

            foreach (var item in items)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                var obj = item.GetAsObject<object>();
                sb.Append(SerialiseAndCompress(obj));
            }

            return sb.ToString();
        }

        public string ToDisplayableResponse(object item)
        {
            return SerialiseAndCompress(item);
        }

        private string SerialiseAndCompress(object item)
        {
            string representation;
            try
            {
                representation = SerializeUsingInstance(item);
            }
            catch (JsonException)
            {
                representation = "serializationError";
            }

            if (IsList(item))
            {
                representation = representation.Replace(",", ", ");
            }
            else if (IsMultilineString(representation))
            {
                representation = SuppressExtraLines(representation);
            }

            return representation;
        }

        private string SerializeUsingInstance(object obj)
        {
            using var stringWriter = new StringWriter();
            var jsonWriter = new JsonTextWriter(stringWriter)
            {
                Formatting = Formatting.None,
            };
            jsonSerializer.Serialize(jsonWriter, obj);
            return stringWriter.ToString();
        }


        private static bool IsList(object item)
        {
            if (item == null) return false;
            bool isJsonArray = item is Newtonsoft.Json.Linq.JArray;
            bool isGenericList = item.GetType().IsGenericType && item.GetType().GetGenericTypeDefinition() == typeof(List<>);
            return isJsonArray || isGenericList;
        }

        private static bool IsMultilineString(string representation)
        {
            return representation.Contains("\\n");
        }

        private static string SuppressExtraLines(string representation)
        {
            var parts = representation.Split(new[] { "\\n" }, StringSplitOptions.None);
            representation = parts[0];

            int suppressedParts = parts.Length - 1;
            representation += $" .. ( {suppressedParts} more line";
            if (suppressedParts > 1)
            {
                representation += "s";
            }
            representation += " )\"";

            return representation;
        }

    }
}
