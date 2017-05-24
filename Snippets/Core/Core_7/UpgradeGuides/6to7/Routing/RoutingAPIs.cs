﻿// ReSharper disable UnusedParameter.Local

namespace Core_7.UpgradeGuides._6to7.Routing
{
    using Core6;
    using NServiceBus;

    class RoutingAPIs
    {
        void LogicalRouting(EndpointConfiguration endpointConfiguration)
        {
            #region 6to7endpoint-mapping-Routing-Logical

            var transportExtensions = endpointConfiguration.UseTransport<MyTransport>();

            var routing = transportExtensions.Routing();
            routing.RouteToEndpoint(
                assembly: typeof(AcceptOrder).Assembly,
                destination: "Sales");

            routing.RouteToEndpoint(
                assembly: typeof(AcceptOrder).Assembly,
                @namespace: "PriorityMessages",
                destination: "Preferred");

            routing.RouteToEndpoint(
                messageType: typeof(SendOrder),
                destination: "Sending");

            #endregion
        }

        void SubscribeRouting(EndpointConfiguration endpointConfiguration)
        {
            #region 6to7endpoint-mapping-Routing-RegisterPublisher

            var transportExtensions = endpointConfiguration.UseTransport<MyTransport>();

            var routing = transportExtensions.Routing();
            routing.RegisterPublisher(
                assembly: typeof(OrderAccepted).Assembly,
                publisherEndpoint: "Sales");

            routing.RegisterPublisher(
                assembly: typeof(OrderAccepted).Assembly,
                @namespace: "PriorityMessages",
                publisherEndpoint: "Preferred");

            routing.RegisterPublisher(
                eventType: typeof(OrderSent),
                publisherEndpoint: "Sending");
            #endregion
        }

        class AcceptOrder
        {
        }

        class SendOrder
        {
        }

        class OrderSent
        {
        }

        class OrderAccepted
        {
        }
    }
}