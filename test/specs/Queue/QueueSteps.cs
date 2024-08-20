using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using TDL.Client;
using TDL.Client.Audit;
using TDL.Client.Queue;
using TDL.Test.Specs.Queue.Factories;
using TDL.Test.Specs.Queue.SpecItems;
using TDL.Test.Specs.Utils.Jmx.Broker;
using TDL.Test.Specs.Utils.Logging;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace TDL.Test.Specs.Queue
{
    [Binding]
    public class QueueSteps
    {
        private const string Hostname = "localhost";
        private const int Port = 21616;
        private const string RequestQueueName = "some-user-req";
        private const string ResponseQueueName = "some-user-resp";

        private readonly LogAuditStream auditStream = new(new ConsoleAuditStream());
        private readonly RemoteJmxBroker broker = TestBroker.Instance;

        private RemoteJmxQueue requestQueue = new(JolokiaSession.Connect("unset", 7777), "unset", "unset");
        private RemoteJmxQueue responseQueue = new(JolokiaSession.Connect("unset", 7777), "unset", "unset");
        private QueueBasedImplementationRunner? queueBasedImplementationRunner;
        private QueueBasedImplementationRunner.Builder queueBasedImplementationRunnerBuilder = new();

        private long requestCount;
        private long processingTimeMillis = 0;

        [Given(@"^I start with a clean broker having a request and a response queue$")]
        public void GivenIStartWithACleanBroker()
        {
            requestQueue = broker.AddQueue(RequestQueueName);
            requestQueue.Purge();

            responseQueue = broker.AddQueue(ResponseQueueName);
            responseQueue.Purge();

            auditStream.ClearLog();
        }

        [Given(@"^a client that connects to the queues$")]
        public void AndAClientThatConnectsToTheQueues()
        {
            var config = new ImplementationRunnerConfig()
                .SetHostname(Hostname)
                .SetPort(Port)
                .SetRequestQueueName(RequestQueueName)
                .SetResponseQueueName(ResponseQueueName)
                .SetAuditStream(auditStream);

            queueBasedImplementationRunnerBuilder = new QueueBasedImplementationRunner.Builder().SetConfig(config);
            queueBasedImplementationRunner = queueBasedImplementationRunnerBuilder.Create();
        }

        [Given(@"^the broker is not available$")]
        public void GivenTheBrokerIsNotAvailable()
        {
            auditStream.ClearLog();

            var config = new ImplementationRunnerConfig()
                .SetHostname("111")
                .SetPort(Port)
                .SetRequestQueueName("X")
                .SetResponseQueueName("Y")
                .SetAuditStream(auditStream);

            queueBasedImplementationRunnerBuilder = new QueueBasedImplementationRunner.Builder().SetConfig(config);
            queueBasedImplementationRunner = queueBasedImplementationRunnerBuilder.Create();
        }

        [Given(@"I receive the following requests:")]
        public void GivenIReceiveTheFollowingRequests(Table table)
        {
            var requests = table.CreateSet<PayloadSpecItem>().Select(i => i.Payload).ToList();

            requests.ForEach(requestQueue.SendTextMessage);
            requestCount = requests.Count;
        }

        [Given(@"^I receive (\d+) identical requests like:$")]
        public void SentIdenticalRequests(int number, Table table)
        {
            var requests = Enumerable
                .Repeat(table.CreateSet<PayloadSpecItem>().Select(p => p.Payload), number)
                .SelectMany(p => p)
                .ToList();

            requests.ForEach(requestQueue.SendTextMessage);
            requestCount = requests.Count;
        }

        [When(@"I go live with the following processing rules:")]
        public void WhenIGoLiveWithTheFollowingProcessingRules(Table table)
        {
            var processingRuleSpecItems = table.CreateSet<ProcessingRuleSpecItem>().ToList();

            processingRuleSpecItems.ForEach(ruleSpec =>
                queueBasedImplementationRunnerBuilder.WithSolutionFor(
                    ruleSpec.Method,
                    CallImplementationFactory.Get(ruleSpec.Call)
                )
            );

            queueBasedImplementationRunner = queueBasedImplementationRunnerBuilder.Create();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            queueBasedImplementationRunner.Run();
            stopwatch.Stop();

            processingTimeMillis = stopwatch.ElapsedMilliseconds;
        }

        [Then(@"^the time to wait for requests is (\d+)ms$")]
        public void ThenTheTimeToWaitForRequestsIs(int expectedTimeout)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.That(queueBasedImplementationRunner.RequestTimeoutMilliseconds, Is.EqualTo(expectedTimeout),
                "The client request timeout has a different value.");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        [Then(@"^the request queue is ""([^""]*)""$")]
        public void ThenTheRequestQueueIs(string expectedName)
        {
            Assert.That(requestQueue.Name, Is.EqualTo(expectedName),
                "Request queue has a different value.");
        }

        [Then(@"^the response queue is ""([^""]*)""$")]
        public void ThenTheResponseQueyeIs(string expectedName)
        {
            Assert.That(responseQueue.Name, Is.EqualTo(expectedName),
                "Response queue has a different value.");
        }

        [Then(@"the client should consume all requests")]
        public void ThenTheClientShouldConsumeAllRequests()
        {
            Assert.That(requestQueue.GetSize(), Is.EqualTo(0),
                "Requests have not been consumed.");
        }

        [Then(@"the client should publish the following responses:")]
        public void ThenTheClientShouldPublishTheFollowingResponses(Table table)
        {
            var expectedResponses = table.CreateSet<PayloadSpecItem>().Select(i => i.Payload).ToList();
            var actualResponses = responseQueue.GetMessageContents();
            Assert.That(expectedResponses.SequenceEqual(actualResponses), Is.True,
                "The responses are not correct");
        }

        [Then(@"the client should not consume any request")]
        public void ThenTheClientShouldNotConsumeAnyRequest()
        {
            Assert.That(requestQueue.GetSize(), Is.EqualTo(requestCount),
                "The request queue has different size. The message has been consumed.");
        }

        [Then(@"the client should not publish any response")]
        public void ThenTheClientShouldNotPublishAnyResponse()
        {
            Assert.That(responseQueue.GetSize(), Is.EqualTo(0),
                "The response queue has different size. Messages have been published.");
        }

        [Then(@"the client should consume one request")]
        public void ThenTheClientShouldConsumeOneRequest()
        {
            Assert.That(requestQueue.GetSize(), Is.EqualTo(requestCount - 1),
                "The request queue has different size. More than one messages have been consumed.");
        }

        [Then(@"the client should consume first request")]
        public void ThenTheClientShouldConsumeFirstRequest()
        {
            Assert.That(requestQueue.GetSize(), Is.EqualTo(requestCount - 1),
                "Wrong number of requests have been consumed.");
        }

        [Then(@"the client should publish one response")]
        public void ThenTheClientShouldPublishOneResponse()
        {
            Assert.That(responseQueue.GetSize(), Is.EqualTo(requestCount - 2),
                "Wrong number of responses have been received.");
        }

        [Then(@"the client should display to console:")]
        public void ThenTheClientShouldDisplayToConsole(Table table)
        {
            var expectedOutputs = table.CreateSet<OutputSpecItem>().ToList();
            var actualOutput = auditStream.GetLog();

            expectedOutputs.ForEach(expectedLine =>
            {
                Assert.That(actualOutput, Does.Contain(expectedLine.Output));
            });
        }

        [Then(@"I should get no exception")]
        public void ThenIShouldGetNoException()
        {
            // No exceptions.
        }

        [Then(@"^the processing time should be lower than (\d+)ms$")]
        public void ProccessingTimeShouldBeLowerThanMs(long threshold)
        {
            Assert.That(processingTimeMillis,  Is.LessThan(threshold));
        }
    }
}
