using TDL.Client.Audit;
using TDL.Client.Queue;

namespace TDL.Test.Specs.Queue.Runners
{
    internal class NoisyImplementationRunner : IImplementationRunner
    {
        private readonly string deployMessage;
        private readonly IAuditStream auditStream;

        public NoisyImplementationRunner(string deployMessage, IAuditStream auditStream)
        {
            this.deployMessage = deployMessage;
            this.auditStream = auditStream;
        }

        public void Run()
        {
            auditStream.WriteLine(deployMessage);
        }
    }
}
