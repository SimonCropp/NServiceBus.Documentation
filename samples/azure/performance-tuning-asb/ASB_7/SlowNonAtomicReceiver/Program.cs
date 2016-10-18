﻿using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

class Program
{
    public static ReceiveCounter ReceiveCounter = new ReceiveCounter();
    static ILog logger = LogManager.GetLogger<Program>();

    static void Main()
    {
        MainAsync().GetAwaiter().GetResult();
    }

    static async Task MainAsync()
    {
        Console.Title = "Samples.ASB.Performance.SlowNonAtomicReceiver";

        ReceiveCounter.Subscribe(i => logger.Warn($"Process {i} messages per second"));

        var endpointConfiguration = new EndpointConfiguration("Samples.ASB.Performance.Receiver");
        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
        var connectionString = Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("Could not read the 'AzureServiceBus.ConnectionString' environment variable. Check the sample prerequisites.");
        }
        transport.ConnectionString(connectionString);
        var topology = transport.UseTopology<ForwardingTopology>();

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();

        #region slow-non-atomic-receiver-config

        transport.Transactions(TransportTransactionMode.ReceiveOnly);
        var queues = transport.Queues();
        queues.EnablePartitioning(true);
        endpointConfiguration.LimitMessageProcessingConcurrencyTo(1);
        var receivers = transport.MessageReceivers();
        receivers.PrefetchCount(0);

        var factories = transport.MessagingFactories();
        factories.NumberOfMessagingFactoriesPerNamespace(1);
        transport.NumberOfClientsPerEntity(1);

        #endregion

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        Console.WriteLine("Receiver is ready to receive messages");
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }

}