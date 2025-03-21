using System;
using Apache.NMS;
using Newtonsoft.Json;
using TDL.Client.Queue.Abstractions;
using TDL.Client.Queue.Abstractions.Response;
using TDL.Client.Queue.Serialization;
using TDL.Client.Utils;

namespace TDL.Client.Queue.Transport
{
    public class RemoteBroker : IRemoteBroker
    {
        private readonly IConnection connection;
        private readonly ISession session;
        private readonly IMessageConsumer messageConsumer;
        private readonly IMessageProducer messageProducer;
        private readonly long timeout;
        private readonly JsonSerializer jsonSerializer;
        private readonly ISerializationProvider serializationProvider;

        public RemoteBroker(
            string hostname,
            int port,
            string requestqueuename,
            string responsequeuename,
            long timeout,
            JsonSerializer jsonSerializer)
        {
            this.timeout = timeout;
            this.jsonSerializer = jsonSerializer;

            var brokerUrl = new Uri($"tcp://{hostname}:{port}");
            var connectionFactory = new Apache.NMS.ActiveMQ.ConnectionFactory(brokerUrl);

            connection = connectionFactory.CreateConnection();
            session = connection.CreateSession(AcknowledgementMode.ClientAcknowledge);

            messageConsumer = session.CreateConsumer(session.GetQueue(requestqueuename));
            messageProducer = session.CreateProducer(session.GetQueue(responsequeuename));

            connection.Start();

            messageProducer.DeliveryMode = MsgDeliveryMode.NonPersistent;

            serializationProvider = new JsonRpcSerializationProvider(jsonSerializer);
        }

        public Maybe<Request> Receive()
        {
            var textMessage = (ITextMessage) messageConsumer.Receive(TimeSpan.FromMilliseconds(timeout));
            
            Request? request = serializationProvider.Deserialize(textMessage);

            if (request == null)
            {
                return Maybe<Request>.None;
            }
            else
            {
                return Maybe<Request>.Some(request);
            }
        }

        public void RespondTo(Request request, IResponse response)
        {
            var serializedResponse = serializationProvider.Serialize(response);

            var textMessage = session.CreateTextMessage(serializedResponse);
            messageProducer.Send(textMessage);

            request.TextMessage?.Acknowledge();
        }

        public void Dispose()
        {
            connection?.Dispose();
            session?.Dispose();
            messageConsumer?.Dispose();
            messageProducer?.Dispose();
        }
    }
}
