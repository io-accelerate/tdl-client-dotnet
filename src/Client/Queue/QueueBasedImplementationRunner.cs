using System;
using TDL.Client.Queue;
using TDL.Client.Queue.Abstractions;
using TDL.Client.Queue.Transport;
using TDL.Client.Utils;
using TDL.Client.Queue.Abstractions.Response;

namespace TDL.Client
{
    public partial class QueueBasedImplementationRunner : IImplementationRunner
    {
        private readonly Audit audit;
        private readonly ProcessingRules deployProcessingRules;
        private readonly ImplementationRunnerConfig config;

        public QueueBasedImplementationRunner(
            ImplementationRunnerConfig config,
            ProcessingRules deployProcessingRules)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.deployProcessingRules = deployProcessingRules ?? throw new ArgumentNullException(nameof(deployProcessingRules));

            if (string.IsNullOrWhiteSpace(config.Hostname))
            {
                throw new ArgumentException("Hostname cannot be null or empty", nameof(config.Hostname));
            }
            
            if (string.IsNullOrWhiteSpace(config.RequestQueueName))
            {
                throw new ArgumentException("RequestQueueName cannot be null or empty", nameof(config.RequestQueueName));
            }

            if (string.IsNullOrWhiteSpace(config.ResponseQueueName))
            {
                throw new ArgumentException("ResponseQueueName cannot be null or empty", nameof(config.ResponseQueueName));
            }

            audit = new Audit(config.AuditStream);
        }

        public long RequestTimeoutMilliseconds => config.RequestTimeoutMilliseconds;

        public void Run()
        {
            audit.LogLine("Starting client");

            try
            {
                using (var remoteBroker = new RemoteBroker(
                    config.Hostname,
                    config.Port,
                    config.RequestQueueName,
                    config.ResponseQueueName,
                    config.RequestTimeoutMilliseconds))
                {
                    audit.LogLine("Waiting for requests");
                    var request = remoteBroker.Receive();
                    while (request.HasValue)
                    {
                        request = ApplyProcessingRules(request.Value, deployProcessingRules, remoteBroker);
                    }
                }
            }
            catch (Exception ex)
            {
                audit.LogException("There was a problem processing messages", ex);
            }

            audit.LogLine("Stopping client");
        }

        private Maybe<Request> ApplyProcessingRules(
            Request request,
            ProcessingRules processingRules,
            RemoteBroker remoteBroker)
        {
            audit.StartLine();
            audit.Log(request);

            var response = processingRules.GetResponseFor(request);
            audit.Log(response);

            audit.EndLine();

            AfterResponse(remoteBroker, request, response);

            return GetNextRequest(remoteBroker, response);
        }

        private void AfterResponse(RemoteBroker remoteBroker, Request request, IResponse response)
        {
            if (response is FatalErrorResponse) {
               // Do nothing
            }
            else {
                remoteBroker.RespondTo(request, response);
            }
        }

        private Maybe<Request> GetNextRequest(RemoteBroker remoteBroker, IResponse response) {
            if (response is FatalErrorResponse) {
               return Maybe<Request>.None;
            }
            else {
               return remoteBroker.Receive();
            }
        }
    }
}
