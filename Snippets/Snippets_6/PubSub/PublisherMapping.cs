﻿namespace Snippets6.PubSub
{
    using NServiceBus;

    class PublisherMapping
    {
        public void MapPublishers()
        {
            #region PubSub-CodePublisherMapping
            BusConfiguration busConfiguration = new BusConfiguration();
            busConfiguration.Pubishers().AddStatic(new Endpoint("Sales"), typeof(MyEvent));
            busConfiguration.Pubishers().AddStatic(new Endpoint("Sales"), typeof(OtherEvent).Assembly);
            #endregion
        }
    }

    public class MyEvent : IEvent
    {
    }

    public class OtherEvent : IEvent
    {
    }
}
