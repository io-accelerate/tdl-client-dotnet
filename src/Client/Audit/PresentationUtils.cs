using System;
using System.Globalization;
using System.Collections;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TDL.Client.Audit
{
    public static class PresentationExtensions
    {
        public static string ToDisplayableRequest(this List<JToken> items)
        {
            StringBuilder sb = new StringBuilder();
            foreach (JToken item in items) 
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                String representation;

                if (item.GetType().Equals(typeof(JArray)))
                {
                    representation = ((JArray)item).ToString(Formatting.None);
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

        public static string ToDisplayableResponse(this Object item)
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
