using System.Text;
using TDL.Client.Audit;

namespace TDL.Test.Specs.Utils.Logging
{
    public class LogAuditStream : IAuditStream
    {
        private readonly StringBuilder log = new StringBuilder();
        private readonly IAuditStream originalStream;

        public LogAuditStream(IAuditStream originalStream)
        {
            this.originalStream = originalStream;
        }

        public void ClearLog()
        {
            log.Clear();
        }

        public string GetLog() => log.ToString();

        public void WriteLine(string value)
        {
            log.AppendLine(value);
            originalStream.WriteLine(value);
        }
    }
}
