﻿using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using NServiceBus;

public class EndpointCommunicationListener :
    ICommunicationListener
{
    StatefulServiceContext context;
    IReliableStateManager stateManager;
    IEndpointInstance endpointInstance;
    EndpointConfiguration endpointConfiguration;

    public EndpointCommunicationListener(StatefulServiceContext context, IReliableStateManager stateManager)
    {
        this.context = context;
        this.stateManager = stateManager;
    }

    public async Task<string> OpenAsync(CancellationToken cancellationToken)
    {
        Logger.Log = m => ServiceEventSource.Current.ServiceMessage(context, m);

        var partitionInfo = await ServicePartitionQueryHelper.QueryServicePartitions(context.ServiceName, context.PartitionId)
            .ConfigureAwait(false);

        endpointConfiguration = new EndpointConfiguration("ZipCodeVoteCount");

        endpointConfiguration.ApplyCommonConfiguration(stateManager);

        #region ApplyPartitionConfigurationToEndpoint-ZipCodeVoteCount

        endpointConfiguration.RegisterPartitionsForThisEndpoint(
            localPartitionKey: partitionInfo.LocalPartitionKey, 
            allPartitionKeys: partitionInfo.Partitions);

        #endregion

       

        return null;
    }

    public async Task Run()
    {
        if (endpointConfiguration != null)
        {
            endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
        }
    }

    public Task CloseAsync(CancellationToken cancellationToken)
    {
        return endpointInstance.Stop();
    }

    public void Abort()
    {
        // Fire & Forget Close
        CloseAsync(CancellationToken.None);
    }

    
}