using Apache.NMS;
using TDL.Client.Queue.Abstractions;
using TDL.Client.Queue.Abstractions.Response;
using TDL.Client.Queue.Transport;

namespace TDL.Client.Queue.Serialization
{
    public interface ISerializationProvider
    {
        Request? Deserialize(ITextMessage messageText);

        string Serialize(IResponse response);
    }
}
