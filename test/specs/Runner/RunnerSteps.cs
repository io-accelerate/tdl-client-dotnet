using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TDL.Client.Audit;
using TDL.Client.Queue;
using TDL.Client.Runner;
using TDL.Client.Utils;
using TDL.Test.Specs.Queue.Runners;
using TDL.Test.Specs.Utils.Logging;
using TDL.Test.Specs.Utils.Extensions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace TDL.Test.Specs.Runner
{
    [Binding]
    public class RunnerSteps
    {
        private WiremockProcess challengeServerStub = new("unset", 9999);
        private WiremockProcess recordingServerStub = new("unset", 8888);
        private string challengeHostname = "unset";
        private int port;
        private readonly IAuditStream auditStream = new TestAuditStream();
        private IImplementationRunner implementationRunner = new QuietImplementationRunner();
        private string implementationRunnerMessage = "unset";
        private string journeyId = "unset";
        private readonly TestActionProvider actionProviderCallback = new();

        [Given(@"There is a challenge server running on ""(.*)"" port (.*)")]
        public void GivenThereIsAChallengeServerRunningOnPort(string hostname, int port)
        {
            this.challengeHostname = hostname;
            this.port = port;

            challengeServerStub = new WiremockProcess(hostname, port);
            challengeServerStub.Reset();
        }

        [Given(@"journeyId is ""(.*)""")]
        public void GivenJourneyIdIs(string journeyId)
        {
            this.journeyId = journeyId;
        }

        [Given(@"the challenge server exposes the following endpoints")]
        public void GivenTheChallengeServerExposesTheFollowingEndpoints(Table table)
        {
            table.CreateSet<ServerConfig>()
                .ToList()
                .ForEach(config => challengeServerStub.CreateNewMapping(config));
        }

        [Given(@"There is a recording server running on ""(.*)"" port (.*)")]
        public void GivenThereIsARecordingServerRunningOnPort(string hostname, int port)
        {
            recordingServerStub = new WiremockProcess(hostname, port);
            recordingServerStub.Reset();
        }

        [Given(@"the recording server exposes the following endpoints")]
        public void GivenTheRecordingServerExposesTheFollowingEndpoints(Table table)
        {
            table.CreateSet<ServerConfig>()
                .ToList()
                .ForEach(config => recordingServerStub.CreateNewMapping(config));
        }

        [Given(@"the action input comes from a provider returning ""(.*)""")]
        public void GivenTheActionInputComesFromAProviderReturning(string s)
        {
            actionProviderCallback.Set(s);

            // WARNING!! very dirty hack - add the lastChallengeRound to challenges folder
            // The proper fix is to make this an explicit Spec step and update all the clients
            var challengesFolder = Path.Combine(PathHelper.RepositoryPath, "challenges");
            Directory.CreateDirectory(challengesFolder);
            var lastRoundPath = Path.Combine(challengesFolder, "XR.txt");
            File.WriteAllText(lastRoundPath, "RoundID" + Environment.NewLine + 
                              "If you see this and you wonder what the heck??"+
                              "then you must now that this file is the result of a hack added to "+
                              "the GivenTheActionInputComesFromAProviderReturning step");
        }

        [Given(@"the challenges folder is empty")]
        public void GivenTheChallengesFolderIsEmpty()
        {
            var challengesPath = Path.Combine(PathHelper.RepositoryPath, "challenges");
            if (Directory.Exists(challengesPath))
            {
                var challengesDirectory = new DirectoryInfo(challengesPath);
                challengesDirectory.Empty();
            }
        }

        [Given(@"the current round is ""(.*)""")]
        public void GivenTheCurrentRoundIs(string roundId)
        {
            var ChallengesFolder = Path.Combine(PathHelper.RepositoryPath, "challenges");
            var CurrentRoundPath = Path.Combine(ChallengesFolder, "XR.txt");

            if (!Directory.Exists(ChallengesFolder))
            {
                Directory.CreateDirectory(ChallengesFolder);
            }
            File.WriteAllText(CurrentRoundPath, roundId);
           
        }

        [Given(@"there is an implementation runner that prints ""(.*)""")]
        public void GivenThereIsAnImplementationRunnerThatPrints(string s)
        {
            implementationRunnerMessage = s;
            implementationRunner = new NoisyImplementationRunner(implementationRunnerMessage, auditStream);
        }

        [Given(@"recording server is returning error")]
        public void GivenRecordingServerIsReturningError()
        {
            recordingServerStub.Reset();
        }

        [Given(@"the challenge server returns (.*), response body ""(.*)"" for all requests")]
        public void GivenTheChallengeServerReturnsResponseBodyForAllRequests(int returnCode, string body)
        {
            var config = new ServerConfig
            {
                EndpointMatches = "^(.*)",
                Status = returnCode,
                Verb = "ANY",
                ResponseBody = body
            };
            challengeServerStub.CreateNewMapping(config);
        }

        [Given(@"the challenge server returns (.*) for all requests")]
        public void GivenTheChallengeServerReturnsForAllRequests(int returnCode)
        {
            var config = new ServerConfig
            {
                EndpointMatches = "^(.*)",
                Status = returnCode,
                Verb = "ANY"
            };
            challengeServerStub.CreateNewMapping(config);
        }

        [When(@"user starts client")]
        public void WhenUserStartsClient()
        {
            var config = ChallengeSessionConfig.ForJourneyId(journeyId)
                .WithServerHostname(challengeHostname)
                .WithPort(port)
                .WithColours(true)
                .WithAuditStream(auditStream)
                .WithRecordingSystemShouldBeOn(true);

            ChallengeSession.ForRunner(implementationRunner)
                    .WithConfig(config)
                    .WithActionProvider(actionProviderCallback)
                    .Start();
        }

        [Then(@"the server interaction should look like:")]
        public void ThenTheServerInteractionShouldLookLike(string expectedOutput)
        {
            var total = auditStream?.ToString() ?? "";
            total = total.TrimEnd(Environment.NewLine.ToCharArray()).Replace("\\", "/");
            Assert.That(total, Does.Contain(expectedOutput), "Expected string is not contained in output");
        }

        [Then(@"the file ""(.*)"" should contain")]
        public void ThenTheFileShouldContain(string file, string text)
        {
            var fileFullPath = Path.Combine(PathHelper.RepositoryPath, file);
            var fileContent = File.ReadAllText(fileFullPath);

            text = text.TrimEnd(Environment.NewLine.ToCharArray());

            Assert.That(fileContent, Is.EqualTo(text), "Contents of the file is not what is expected");
        }

        [Then(@"the recording system should be notified with ""(.*)""")]
        public void ThenTheRecordingSystemShouldBeNotifiedWith(string expectedOutput)
        {
            Assert.That(recordingServerStub.EndpointWasHit("/notify", "POST", expectedOutput), Is.True);
        }

        [Then(@"the recording system should have received a stop signal")]
        public void ThenTheRecordingSystemShouldReceiveStopSignal()
        {
            Assert.That(recordingServerStub.EndpointWasHit("/stop", "POST", ""), Is.True);
        }

        [Then(@"the implementation runner should be run with the provided implementations")]
        public void ThenTheImplementationRunnerShouldBeRunWithTheProvidedImplementations()
        {
            var total = auditStream.ToString();
            Assert.That(total, Does.Contain(implementationRunnerMessage));
        }

        [Then(@"the server interaction should contain the following lines:")]
        public void ThenTheServerInteractionShouldContainTheFollowingLines(string expectedOutput)
        {
            var total = auditStream.ToString();
            var lines = expectedOutput.Split('\n');
            foreach (var line in lines)
            {
                Assert.That(total, Does.Contain(line), "Expected string is not contained in output");
            }
        }

        [Then(@"the client should not ask the user for input")]
        public void ThenTheClientShouldNotAskTheUserForInput()
        {
            var total = auditStream.ToString();
            Assert.That(total, Does.Not.Contain("Selected action is:"));
        }
    }
}
