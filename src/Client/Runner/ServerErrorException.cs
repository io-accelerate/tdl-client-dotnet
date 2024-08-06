using System;

namespace TDL.Client.Runner
{
    internal class ServerErrorException : Exception
    {
        public ServerErrorException(string responseStatusDescription)
            : base(responseStatusDescription)
        {
        }
    }
}
