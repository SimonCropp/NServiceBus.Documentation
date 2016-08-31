﻿namespace Endpoint1
{
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Logging;
    using Shared;

    public class LongProcessingFailedHandler : IHandleMessages<LongProcessingFailed>
    {
        static ILog log = LogManager.GetLogger<LongProcessingFailedHandler>();

        public Task Handle(LongProcessingFailed message, IMessageHandlerContext context)
        {
            log.Info($"Request with ID {message.Id} has failed. Reason: {message.Reason}");

            return Task.FromResult(0);
        }
    }
}