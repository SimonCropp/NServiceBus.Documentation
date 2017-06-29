﻿using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Persistence;
using NServiceBus.Persistence.Legacy;
using System;
using System.Threading.Tasks;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            var endpointConfiguration = new EndpointConfiguration("Publisher");
            endpointConfiguration.UseTransport<MsmqTransport>();

            endpointConfiguration.UsePersistence<MsmqPersistence, StorageType.Subscriptions>()
                .SubscriptionQueue("Publisher.Subscriptions");

            endpointConfiguration.DisableFeature<TimeoutManager>();

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine("\r\nBus created and configured; press any key to stop program\r\n");
            Console.ReadKey();

            await endpointInstance.Stop()
                    .ConfigureAwait(false);
        }
    }
}
