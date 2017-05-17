﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

static class Program
{
    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        Console.Title = "Samples.TransientClients.Client";

        Console.WriteLine("Press any key to connect.");
        Console.ReadKey(true);

        using (var hubConnection = new HubConnection("http://localhost:9756"))
        {
            var stockHubProxy = hubConnection.CreateHubProxy("StockTicksHub");
            stockHubProxy.On<StockTick>("PushStockTick",
                onData: stock =>
                {
                    Console.WriteLine($"Stock update for {stock.Symbol} at {stock.Timestamp:O}. Press any key to exit.");
                });
            await hubConnection.Start()
                .ConfigureAwait(false);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(true);

            hubConnection.Stop();
        }
    }
}