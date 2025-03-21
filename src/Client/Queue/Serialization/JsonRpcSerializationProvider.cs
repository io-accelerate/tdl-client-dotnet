using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Apache.NMS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TDL.Client.Queue.Abstractions;
using TDL.Client.Queue.Abstractions.Response;
using TDL.Client.Queue.Transport;

namespace TDL.Client.Queue.Serialization
{
    public class JsonRpcSerializationProvider : ISerializationProvider
    {
        private readonly JsonSerializer jsonSerializer;

        public JsonRpcSerializationProvider(JsonSerializer jsonSerializer)
        {
            this.jsonSerializer = jsonSerializer;
        }

        public Request? Deserialize(ITextMessage textMessage)
        {
             if (textMessage == null)
            {
                return null;
            }

            try
            {
                using (var reader = new StringReader(textMessage.Text))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    JsonRpcRequest jsonRpcRequest = jsonSerializer.Deserialize<JsonRpcRequest>(jsonReader)
                        ?? throw new DeserializationException("Failed to deserialize JSON-RPC request.");


                    List<ParamAccessor> paramAccessors = jsonRpcRequest.Params
                        .Select(jsonNode => new ParamAccessor(jsonNode, jsonSerializer))
                        .ToList();
                    return new Request
                    {
                        TextMessage = textMessage,
                        MethodName = jsonRpcRequest.MethodName,
                        Params = paramAccessors,
                        Id = jsonRpcRequest.Id,
                    };
                }
            }
            catch (JsonReaderException ex)
            {
                throw new DeserializationException("Invalid message format", ex);
            }
        }

        public string Serialize(IResponse response)
        {
            try
            {
                using (var writer = new StringWriter())
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    JsonRpcResponse jsonRpcResponse = new()
                    {
                        Result = response.Result,
                        Error = null,
                        Id = response.Id
                    };

                    jsonSerializer.Serialize(jsonWriter, jsonRpcResponse);
                    return writer.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException("Could not serialize response", ex);
            }
        }
    }
}
