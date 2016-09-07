﻿using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Transport.SQLServer;
#pragma warning disable 618

public class Program
{
    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        Console.Title = "Samples.SqlServer.MultiInstanceSender";

        #region SenderConfiguration

        var endpointConfiguration = new EndpointConfiguration("Samples.SqlServer.MultiInstanceSender");
        var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
        transport.EnableLegacyMultiInstanceMode(ConnectionProvider.GetConnection);
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();

        #endregion

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);

        Console.WriteLine("Press <enter> to send a message");
        Console.WriteLine("Press any other key to exit");
        try
        {
            while (true)
            {
                if (Console.ReadKey().Key != ConsoleKey.Enter)
                {
                    return;
                }
                await PlaceOrder(endpointInstance)
                .ConfigureAwait(false);
            }
        }
        finally
        {
            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }

    static async Task PlaceOrder(IEndpointInstance endpoint)
    {
        #region SendMessage

        var order = new ClientOrder
        {
            OrderId = Guid.NewGuid()
        };
        await endpoint.Send("Samples.SqlServer.MultiInstanceReceiver", order)
            .ConfigureAwait(false);

        #endregion

        Console.WriteLine($"ClientOrder message sent with ID {order.OrderId}");
    }

    #region SenderConnectionProvider
    static class ConnectionProvider
    {
        const string DefaultConnectionString = @"Data Source=.\SqlExpress;Database=SenderCatalog;Integrated Security=True";
        const string ReceiverConnectionString = @"Data Source=.\SqlExpress;Database=ReceiverCatalog;Integrated Security=True";

        public static async Task<SqlConnection> GetConnection(string transportAddress)
        {
            var connectionString = transportAddress.StartsWith("Samples.SqlServer.MultiInstanceReceiver")
                ? ReceiverConnectionString
                : DefaultConnectionString;

            var connection = new SqlConnection(connectionString);

            await connection.OpenAsync().ConfigureAwait(false);

            return connection;
        }
    }
    #endregion

}
