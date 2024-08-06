using System;

namespace TDL.Client.Audit
{
    public class ConsoleAuditStream : IAuditStream
    {
        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }
    }
}
