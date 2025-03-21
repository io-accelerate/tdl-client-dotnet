using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TDL.Client.Queue.Abstractions;


namespace TDL.Client.Audit
{
    public class PresentationUtils
    {
        private readonly JsonSerializer jsonSerializer;

        public PresentationUtils(JsonSerializer jsonSerializer)
        {
            this.jsonSerializer = jsonSerializer;
        }


        public string ToDisplayableRequest(List<ParamAccessor> items)
        {
            StringBuilder sb = new();
            foreach (ParamAccessor item in items) 
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                String representation;

                if (item.IsArray())
                {
                    representation = item.GetAsObject<JArray>().ToString(Formatting.None);
                    representation = representation.Replace(",", ", ");  
                }
                else
                {
                    // Ensure the item's ToString method does not return null
                    representation = item?.ToString() ?? string.Empty;

                   // Only check for numeric if item is not null
                    if (item != null && IsNotNumeric(item))
                        representation = AddQuotes(representation);

                    if (IsMultiLineString(representation))
                        representation = SuppressExtraLines(representation);
                }
                sb.Append(representation);
            }
            return sb.ToString();
        }

        public string ToDisplayableResponse(Object item)
        {
            if (item == null)
                return "null";
            
            var representation = item.ToString() ?? string.Empty;

            if (item.GetType().IsArray || item is IList)
                representation = PrimitiveArrayToString(item);
            else if (IsNotNumeric(item))
                representation = AddQuotes(representation);
            
            if (IsMultiLineString(representation))
                representation = SuppressExtraLines(representation);

            return representation;
        }


        private static bool IsMultiLineString(string value) =>
            value.Contains("\n");

        private static bool IsNotNumeric(object item) =>
            !IsNumeric(item);

        private static bool IsNumeric(object item) =>
            item != null &&
            double.TryParse(Convert.ToString(item, CultureInfo.InvariantCulture), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out double _);

        private static string SuppressExtraLines(string representation)
        {
            string[] parts = representation.Split('\n');
            representation = parts[0];

            int suppressedParts = parts.Length - 1;
            representation += " .. ( " + suppressedParts + " more line";

            if (suppressedParts > 1)
            {
                representation += "s";
            }

            representation += " )\"";
            return representation;
        }

        private static string AddQuotes(string value) =>
            "\""+value+"\"";

        private static string PrimitiveArrayToString(object array) 
        {
            string json_array_as_string = JsonConvert.SerializeObject(array);
            string representation = json_array_as_string.Replace(",", ", ");
            return representation;
        }

    }
}
