﻿namespace Core8.DelayedDelivery
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus;

    class DeferForTimeSpan
    {
        async Task SendDelayedMessage(EndpointConfiguration endpointConfiguration, IEndpointInstance endpoint, IMessageHandlerContext handlerContext)
        {
            #region configure-persistence-timeout
#pragma warning disable 0618
            endpointConfiguration.UsePersistence<InMemoryPersistence, StorageType.Timeouts>();
#pragma warning restore  0618
            #endregion

            #region delayed-delivery-timespan

            var sendOptions = new SendOptions();

            sendOptions.DelayDeliveryWith(TimeSpan.FromMinutes(30));

            await handlerContext.Send(new MessageToBeSentLater(), sendOptions)
                .ConfigureAwait(false);
            // OR
            await endpoint.Send(new MessageToBeSentLater(), sendOptions)
                .ConfigureAwait(false);

            #endregion
        }

        class MessageToBeSentLater
        {
        }
    }
}