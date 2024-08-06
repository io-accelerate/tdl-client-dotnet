using System;
using System.Text;
using TDL.Client.Audit;

namespace TDL.Client
{
    public partial class QueueBasedImplementationRunner
    {
        private class Audit
        {
            private readonly IAuditStream auditStream;
            private readonly StringBuilder lineBuilder;

            public Audit(IAuditStream auditStream)
            {
                this.auditStream = auditStream;
                lineBuilder = new StringBuilder();
            }

            public void StartLine()
            {
                lineBuilder.Clear();
            }

            public void Log(IAuditable auditable)
            {
                if (string.IsNullOrEmpty(auditable.AuditText))
                {
                    return;
                }

                if (lineBuilder.Length > 0)
                {
                    lineBuilder.Append(", ");
                }

                lineBuilder.Append(auditable.AuditText);
            }

            public void EndLine()
            {
                auditStream.WriteLine(lineBuilder.ToString());
            }

            public void LogException(string message, Exception ex)
            {
                StartLine();
                lineBuilder.Append($"{message}: {ex.Message}");
                EndLine();
            }

            public void LogLine(string text)
            {
                StartLine();
                lineBuilder.Append(text);
                EndLine();
            }
        }
    }
}
