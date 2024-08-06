using System;
using TDL.Client.Queue.Abstractions;
using TDL.Client.Queue.Abstractions.Response;
using TDL.Client.Utils;

namespace TDL.Client.Queue.Transport
{
    public interface IRemoteBroker : IDisposable
    {
        Maybe<Request> Receive();

        void RespondTo(Request request, IResponse response);
    }
}
