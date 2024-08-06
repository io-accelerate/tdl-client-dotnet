using System;

namespace TDL.Client.Runner
{
    internal class OtherCommunicationException : Exception
    {
        public OtherCommunicationException(string responseStatusDescription)
            : base(responseStatusDescription)
        {
        }
    }
}
