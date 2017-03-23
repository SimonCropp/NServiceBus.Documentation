﻿using Microsoft.ServiceFabric.Data;
using NServiceBus;
using NServiceBus.Persistence.ServiceFabric;

class GettingStarted
{
    void GettingStartedUsage(IReliableStateManager statemanager, EndpointConfiguration endpointConfiguration)
    {
        #region ServiceFabricPersistenceConfiguration
        var persistence = endpointConfiguration.UsePersistence<ServiceFabricPersistence>();
        persistence.StateManager(statemanager);
        #endregion
    }


}