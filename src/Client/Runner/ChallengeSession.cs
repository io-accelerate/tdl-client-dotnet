using System;
using TDL.Client.Queue;

namespace TDL.Client.Runner
{
    public class ChallengeSession
    {
        private ChallengeSessionConfig? config;
        private readonly IImplementationRunner implementationRunner;
        private RecordingSystem? recordingSystem;
        private ChallengeServerClient? challengeServerClient;
        private IActionProvider? userInputCallback;

        public static ChallengeSession ForRunner(IImplementationRunner implementationRunner)
        {
            return new ChallengeSession(implementationRunner);
        }

        private ChallengeSession(IImplementationRunner runner)
        {
            implementationRunner = runner;
        }

        public ChallengeSession WithConfig(ChallengeSessionConfig config)
        {
            this.config = config;
            return this;
        }

        public ChallengeSession WithActionProvider(IActionProvider callback)
        {
            userInputCallback = callback;
            return this;
        }

        /// <summary>
        /// The entry point.
        /// </summary>
        public void Start()
        {
            if (config is null) {
                throw new System.InvalidOperationException("Challenge session has not been configured.");
            }

            recordingSystem = new RecordingSystem(config.RecordingSystemShouldBeOn);
            var auditStream = config.AuditStream;

            if (!recordingSystem.IsRecordingSystemOk())
            {
                auditStream.WriteLine("Please run `record_screen_and_upload` before continuing.");
                return;
            }

            WindowsConsoleSupport.EnableColours();

            auditStream.WriteLine("Connecting to " + config.Hostname);
            RunApp();
        }

        private void RunApp()
        {
            if (config is null || recordingSystem is null || userInputCallback is null) {
                throw new System.InvalidOperationException("Challenge session has not been configured.");
            }

            var auditStream = config.AuditStream;
            challengeServerClient = new ChallengeServerClient(config.Hostname, config.Port, config.JourneyId, config.UseColours);

            try
            {
                var journeyProgress = challengeServerClient.GetJourneyProgress();
                auditStream.WriteLine(journeyProgress);

                var availableActions = challengeServerClient.GetAvailableActions();
                auditStream.WriteLine(availableActions);

                bool noActionsAvailable = availableActions.Contains("No actions available.");
                if (noActionsAvailable)
                {
                    recordingSystem.TellToStop();
                    return;
                }

                var userInput = userInputCallback.Get();
                auditStream.WriteLine("Selected action is: " + userInput);

                if (userInput.Equals("deploy"))
                {
                    implementationRunner.Run();
                    var lastFetchedRound = RoundManagement.GetLastFetchedRound();
                    recordingSystem.NotifyEvent(lastFetchedRound, RecordingEvent.ROUND_SOLUTION_DEPLOY);
                }

                var actionFeedback = challengeServerClient.SendAction(userInput);
                if (actionFeedback.Contains("Round time for"))
                {
                    var lastFetchedRound = RoundManagement.GetLastFetchedRound();
                    recordingSystem.NotifyEvent(lastFetchedRound, RecordingEvent.ROUND_COMPLETED);
                }
                if (actionFeedback.Contains("All challenges have been completed"))
                {
                    recordingSystem.TellToStop();
                }

                config.AuditStream.WriteLine(actionFeedback);
                var roundDescription = challengeServerClient.GetRoundDescription();
                RoundManagement.SaveDescription(recordingSystem, roundDescription, auditStream);
            }
            catch (ServerErrorException)
            {
                auditStream.WriteLine("Server experienced an error. Try again in a few minutes.");
            }
            catch (OtherCommunicationException)
            {
                auditStream.WriteLine("Client threw an unexpected error. Try again.");
            }
            catch (ClientErrorException e)
            {
                // The client sent something the server didn't expect.
                auditStream.WriteLine(e.Message);
            }
        }



        private string ExecuteAction(String userInput)
        {
            if (config is null || recordingSystem is null || challengeServerClient is null) {
                throw new System.InvalidOperationException("Challenge session has not been configured.");
            }

            var actionFeedback = challengeServerClient.SendAction(userInput);
            config.AuditStream.WriteLine(actionFeedback);
            if (actionFeedback.Contains("Round time for")) {
                var lastFetchedRound = RoundManagement.GetLastFetchedRound();
                recordingSystem.NotifyEvent(lastFetchedRound, RecordingEvent.ROUND_COMPLETED);
            }

            return challengeServerClient.GetRoundDescription();
        }
    }
}
