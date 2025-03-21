using TDL.Client.Audit;

namespace TDL.Client.Queue.Abstractions.Response
{
    public interface IResponse
    {
        string Id { get; }

        object Result { get; }

        bool IsError { get; }
    }
}
