﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MessagePack.Resolvers;
using NServiceBus;
using NServiceBus.MessagePack;

static class Program
{
    static async Task Main()
    {
        //required to prevent possible occurrence of .NET Core issue https://github.com/dotnet/coreclr/issues/12668
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

        Console.Title = "Samples.Serialization.MessagePack";
        #region config
        var endpointConfiguration = new EndpointConfiguration("Samples.Serialization.MessagePack");
        var serialization = endpointConfiguration.UseSerialization<MessagePackSerializer>();
        serialization.Resolver(ContractlessStandardResolver.Instance);

        #endregion
        endpointConfiguration.UsePersistence<LearningPersistence>();
        endpointConfiguration.UseTransport<LearningTransport>();

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        #region message
        var message = new CreateOrder
        {
            OrderId = 9,
            Date = DateTime.Now,
            CustomerId = 12,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    ItemId = 6,
                    Quantity = 2
                },
                new OrderItem
                {
                    ItemId = 5,
                    Quantity = 4
                },
            }
        };
        await endpointInstance.SendLocal(message)
            .ConfigureAwait(false);
        #endregion
        Console.WriteLine("Order Sent");
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}