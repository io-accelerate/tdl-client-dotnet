using TDL.Client.Audit;

namespace TDL.Client.Runner
{
    public class ChallengeSessionConfig
    {
        public IAuditStream AuditStream;
        public bool RecordingSystemShouldBeOn { get; private set; }
        public string Hostname { get; private set; }
        public int Port { get; private set; }
        public string JourneyId { get; private set; }
        public bool UseColours { get; private set; }

        public static ChallengeSessionConfig ForJourneyId(string journeyId)
        {
            return new ChallengeSessionConfig(journeyId);
        }

        private ChallengeSessionConfig(string journeyId)
        {
            Port = 8222;
            UseColours = true;
            RecordingSystemShouldBeOn = true;
            AuditStream = new ConsoleAuditStream();
            JourneyId = journeyId;
        }

        public ChallengeSessionConfig WithServerHostname(string hostname)
        {
            Hostname = hostname;
            return this;
        }

        public ChallengeSessionConfig WithPort(int port)
        {
            Port = port;
            return this;
        }

        public ChallengeSessionConfig WithColours(bool useColours)
        {
            UseColours = useColours;
            return this;
        }

        public ChallengeSessionConfig WithAuditStream(IAuditStream auditStream)
        {
            AuditStream = auditStream;
            return this;
        }

        public ChallengeSessionConfig WithRecordingSystemShouldBeOn(bool recordingSystemShouldBeOn)
        {
            RecordingSystemShouldBeOn = recordingSystemShouldBeOn;
            return this;
        }
    }
}
