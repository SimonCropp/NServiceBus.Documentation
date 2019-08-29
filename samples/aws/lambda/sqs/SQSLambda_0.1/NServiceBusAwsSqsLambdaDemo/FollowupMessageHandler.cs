﻿using System.Threading.Tasks;
using AwsLambda;
using NServiceBus;
using NServiceBus.Logging;

#region FollowupMessageHandler

public class FollowupMessageHandler : IHandleMessages<FollowupMessage>
{
    private static readonly ILog Log = LogManager.GetLogger<FollowupMessageHandler>();

    public Task Handle(FollowupMessage message, IMessageHandlerContext context)
    {
        Log.Info($"Handling {nameof(FollowupMessage)}.");
        return Task.CompletedTask;
    }
}

#endregion