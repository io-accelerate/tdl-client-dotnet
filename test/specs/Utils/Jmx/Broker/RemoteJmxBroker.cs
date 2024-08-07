using System;
using System.Collections.Generic;

namespace TDL.Test.Specs.Utils.Jmx.Broker
{
    internal class RemoteJmxBroker
    {
        private readonly JolokiaSession jolokiaSession;
        private readonly string brokerName;

        private RemoteJmxBroker(JolokiaSession jolokiaSession, string brokerName)
        {
            this.jolokiaSession = jolokiaSession;
            this.brokerName = brokerName;
        }

        public static RemoteJmxBroker Connect(string hostname, int port, string brokerName)
        {
            try
            {
                var jolokiaSession = JolokiaSession.Connect(hostname, port);
                return new RemoteJmxBroker(jolokiaSession, brokerName);
            }
            catch (Exception e)
            {
                throw new Exception("Broker is busted.", e);
            }
        }

        public RemoteJmxQueue AddQueue(string queueName)
        {
            jolokiaSession.Request(new Dictionary<string, object>
            {
                ["type"] = "exec",
                ["mbean"] = $"org.apache.activemq:type=Broker,brokerName={brokerName}",
                ["operation"] = "addQueue",
                ["arguments"] = new List<string> {queueName}
            });
            return new RemoteJmxQueue(jolokiaSession, brokerName, queueName);
        }
    }
}
