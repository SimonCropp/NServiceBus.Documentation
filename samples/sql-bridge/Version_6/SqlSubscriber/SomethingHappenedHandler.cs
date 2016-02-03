﻿

#region sqlsubscriber-handler
namespace SqlSubscriber
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus;
    using Shared;

    class SomethingHappenedHandler : IHandleMessages<SomethingHappened>
    {
        public async Task Handle(SomethingHappened message, IMessageHandlerContext context)
        {
            Console.WriteLine("Sql Subscriber has now received this event from the SqlBridge.");

            // You can now relay this event to other interested SQL subscribers
            await context.Publish(message);
        }
    }
}
#endregion