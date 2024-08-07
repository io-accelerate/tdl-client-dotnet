using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;

namespace TDL.Test.Specs.Utils.Extensions
{
    internal static class SpecFlowTableExtensions
    {
        /// <summary>
        /// Converts a single columned table to a list of strings.
        /// </summary>
        public static List<string> ToList(this Table table) =>
            table.Rows
                .Select(row => row.First().Value)
                .ToList();
    }
}
