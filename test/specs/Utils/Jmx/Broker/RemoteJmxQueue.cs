﻿using System.Collections.Generic;
using System.Linq;
using TDL.Test.Specs.Utils.Jmx.Broker.JolokiaResponses;

namespace TDL.Test.Specs.Utils.Jmx.Broker
{
    internal class RemoteJmxQueue
    {
        private readonly JolokiaSession jolokiaSession;
        private readonly string queueBean;

        public string Name { get; private set; }

        public RemoteJmxQueue(JolokiaSession jolokiaSession, string brokerName, string queueName)
        {
            this.jolokiaSession = jolokiaSession;
            queueBean = $"org.apache.activemq:type=Broker,brokerName={brokerName},destinationType=Queue,destinationName={queueName}";
            Name = queueName;
        }

        public void SendTextMessage(string message)
        {
            jolokiaSession.Request(new Dictionary<string, object>
            {
                ["type"] = "exec",
                ["mbean"] = queueBean,
                ["operation"] = "sendTextMessage(java.lang.String)",
                ["arguments"] = new List<string> {message}
            });
        }

        public long GetSize()
        {
            var response = jolokiaSession.Request<long>(new Dictionary<string, object>
            {
                ["type"] = "read",
                ["mbean"] = queueBean,
                ["attribute"] = "QueueSize"
            });

            return response.Value;
        }

        public void Purge()
        {
            jolokiaSession.Request(new Dictionary<string, object>
            {
                ["type"] = "exec",
                ["mbean"] = queueBean,
                ["operation"] = "purge()"
            });
        }

        public List<string> GetMessageContents()
        {
            var response = jolokiaSession.Request<List<JolokiaBrowseResponseValueItem>>(new Dictionary<string, object>
            {
                ["type"] = "exec",
                ["mbean"] = queueBean,
                ["operation"] = "browse()"
            });
            // Ensure response.Value is not null before processing it
            if (response?.Value == null)
            {
                return new List<string>();
            }

            // Filter out any null items in the Text property and return a List<string>
            return response.Value
                .Where(static i => i.Text != null)
                .Select(static i => i.Text!)
                .ToList();
                }
    }
}
