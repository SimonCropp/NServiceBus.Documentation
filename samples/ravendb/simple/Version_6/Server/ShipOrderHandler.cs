﻿using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.RavenDB.Persistence;

#region handler    

public class ShipOrderHandler : IHandleMessages<ShipOrder>
{
    IAsyncSessionProvider sessionProvider;

    public ShipOrderHandler(IAsyncSessionProvider sessionProvider)
    {
        this.sessionProvider = sessionProvider;
    }

    public async Task Handle(ShipOrder message, IMessageHandlerContext context)
    {
        var session = sessionProvider.AsyncSession;
        await session.StoreAsync(new OrderShipped
        {
            Id = message.OrderId,
            ShippingDate = DateTime.UtcNow,
        });
    }
}

#endregion