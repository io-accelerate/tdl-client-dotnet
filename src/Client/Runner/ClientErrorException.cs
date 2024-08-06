using System;

namespace TDL.Client.Runner
{
    internal class ClientErrorException : Exception
    {
        public ClientErrorException(string responseContent)
            : base(responseContent)
        {
        }
    }
}
