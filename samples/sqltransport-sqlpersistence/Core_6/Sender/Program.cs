﻿using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Transport.SQLServer;
using NServiceBus.Persistence.Sql;

public static class Program
{
    static Random random;

    public static void Main()
    {
        random = new Random();
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        Console.Title = "Samples.Sql.Sender";
        var endpointConfiguration = new EndpointConfiguration("Samples.Sql.Sender");
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();

        #region SenderConfiguration

        var connection = @"Data Source=.\SqlExpress;Database=shared;Integrated Security=True";
        var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
        transport.ConnectionString(connection);
        transport.DefaultSchema("sender");
        transport.UseSchemaForQueue("error", "dbo");
        transport.UseSchemaForQueue("audit", "dbo");

        var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
        persistence.SqlVariant(SqlVariant.MsSqlServer);
        persistence.ConnectionBuilder(
            connectionBuilder: () =>
            {
                return new SqlConnection(connection);
            });
        persistence.Schema("sender");
        persistence.TablePrefix("");
        var subscriptions = persistence.SubscriptionSettings();
        subscriptions.CacheFor(TimeSpan.FromMinutes(1));

        #endregion

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        Console.WriteLine("Press enter to send a message");
        Console.WriteLine("Press any key to exit");

        while (true)
        {
            var key = Console.ReadKey();
            Console.WriteLine();

            if (key.Key != ConsoleKey.Enter)
            {
                break;
            }

            var orderSubmitted = new OrderSubmitted
            {
                OrderId = Guid.NewGuid(),
                Value = random.Next(100)
            };
            await endpointInstance.Publish(orderSubmitted)
                .ConfigureAwait(false);
            Console.WriteLine("Published OrderSubmitted message");
        }
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}