﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using NServiceBus;

class Program
{

    const int NumberOfMessages = 10000;

    static void Main()
    {
        MainAsync().GetAwaiter().GetResult();
    }

    static async Task MainAsync()
    {
        Console.Title = "Samples.ASB.Performance.FastSender";
        var endpointConfiguration = new EndpointConfiguration("Samples.ASB.Performance.Sender");
        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
        var connectionString = Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("Could not read the 'AzureServiceBus.ConnectionString' environment variable. Check the sample prerequisites.");
        }
        transport.UseTopology<ForwardingTopology>();
        transport.ConnectionString(connectionString);
        var queues = transport.Queues();
        queues.EnablePartitioning(true);

        var receiverName = "Samples.ASB.Performance.Receiver";
        await EnsureReceiverQueueExists(receiverName, connectionString)
            .ConfigureAwait(false);
        var routing = transport.Routing();
        routing.RouteToEndpoint(typeof(SomeMessage), receiverName);

        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo("error");

        #region fast-send-config

        var factories = transport.MessagingFactories();
        factories.BatchFlushInterval(TimeSpan.FromMilliseconds(100));
        factories.NumberOfMessagingFactoriesPerNamespace(5);
        transport.NumberOfClientsPerEntity(5);
        #endregion

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);

        try
        {
            Console.WriteLine("Press 'e' to send a large amount of messages");
            Console.WriteLine("Press any other key to exit");

            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                var eventId = Guid.NewGuid();

                if (key.Key != ConsoleKey.E)
                {
                    break;
                }

                var stopwatch = Stopwatch.StartNew();

                #region fast-send

                var tasks = new List<Task>();
                for (var i = 0; i < NumberOfMessages; i++)
                {
                    var task = endpointInstance.Send(new SomeMessage());
                    tasks.Add(task);
                }

                Console.WriteLine("Waiting for completion...");
                // by awaiting the sends as one unit, this code allows the ASB SDK's client side batching to kick in and bundle sends
                // this results in less latency overhead per individual sends and thus higher performance
                await Task.WhenAll(tasks)
                    .ConfigureAwait(false);

                #endregion

                stopwatch.Stop();
                var elapsedSeconds = stopwatch.ElapsedTicks/(double) Stopwatch.Frequency;
                var msgsPerSecond = NumberOfMessages/elapsedSeconds;
                Console.WriteLine($"sending {NumberOfMessages} messages took {stopwatch.ElapsedMilliseconds} milliseconds, or {msgsPerSecond} messages per second");
            }
        }
        finally
        {
            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }

    static async Task EnsureReceiverQueueExists(string receiverPath, string connectionString)
    {
        var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
        if (!await namespaceManager.QueueExistsAsync(receiverPath).ConfigureAwait(false))
        {
            var queueDescription = new QueueDescription(receiverPath)
            {
                EnablePartitioning = true,
                EnableBatchedOperations = true
            };
            await namespaceManager.CreateQueueAsync(queueDescription)
                .ConfigureAwait(false);
        }
    }
}