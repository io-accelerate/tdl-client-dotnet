using TDL.Test.Specs.Utils.Jmx.Broker;

namespace TDL.Test.Specs
{
    internal static class TestBroker
    {
        private const string Hostname = "localhost";
        private const int JmxPort = 28161;
        private const string BrokerName = "TEST.BROKER";

        public static RemoteJmxBroker Instance { get; }

        static TestBroker()
        {
            if (Instance == null)
            {
                Instance = RemoteJmxBroker.Connect(Hostname, JmxPort, BrokerName);
            }
        }
    }
}
