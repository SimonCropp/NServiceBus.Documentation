﻿using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

class Program
{
    static async Task Main()
    {
        //required to prevent possible occurrence of .NET Core issue https://github.com/dotnet/coreclr/issues/12668
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

        var defaultFactory = LogManager.Use<DefaultFactory>();
        defaultFactory.Level(LogLevel.Warn);

        Console.Title = "Samples.RenameSaga.Version1";

        var endpointConfiguration = new EndpointConfiguration("Samples.RenameSaga");

        SharedConfiguration.Apply(endpointConfiguration);

        endpointConfiguration.PurgeOnStartup(true);
        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);

        Console.WriteLine("Version1 of Sagas starting. Will exit in 5 seconds. After exist start Phase 2 Endpoint.");

        #region startSagas
        var startReplySaga = new StartReplySaga
        {
            TheId = Guid.NewGuid()
        };
        await endpointInstance.SendLocal(startReplySaga)
            .ConfigureAwait(false);

        var startTimeoutSaga = new StartTimeoutSaga
        {
            TheId = Guid.NewGuid()
        };
        await endpointInstance.SendLocal(startTimeoutSaga)
            .ConfigureAwait(false);
        #endregion

        await Task.Delay(TimeSpan.FromSeconds(5))
            .ConfigureAwait(false);
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}