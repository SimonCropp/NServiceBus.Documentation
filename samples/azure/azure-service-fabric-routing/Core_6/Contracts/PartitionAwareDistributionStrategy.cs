using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Configuration.AdvanceExtensibility;
using NServiceBus.Features;
using NServiceBus.Pipeline;
using NServiceBus.Transport;

namespace Contracts
{
    using System;
    using System.Linq;
    using NServiceBus;
    using NServiceBus.Routing;

    public abstract class PartitionAwareDistributionStrategy : DistributionStrategy
    {
        protected PartitionAwareDistributionStrategy(string endpoint, DistributionStrategyScope scope) : base(endpoint, scope)
        {
        }

        public override string SelectReceiver(string[] receiverAddresses)
        {
            throw new NotImplementedException();
        }

        protected abstract string MapMessageToPartition(object message);

        public override string SelectDestination(DistributionContext context)
        {
            var discriminator = MapMessageToPartition(context.Message.Instance);

            // stamp message with the partition key so that behavior used for receiver side can identify the message destination
            context.Headers[PartitionHeaders.PartitionKey] = discriminator;

            var logicalAddress = LogicalAddress.CreateRemoteAddress(new EndpointInstance(Endpoint, discriminator));
            return context.ReceiverAddresses.Single(a => a == logicalAddress.ToString());
        }
    }

    public static class EndpointConfigurationExtensions
    {
        public static void EnableReceiverSideDistribution(this EndpointConfiguration configuration, HashSet<string> discriminators)
        {
            configuration.GetSettings().Set("ReceiverSideDistribution.Discriminators", discriminators);
            configuration.EnableFeature<ReceiverSideDistribution>();
        }
    }

    class ReceiverSideDistribution : Feature
    {
        protected override void Setup(FeatureConfigurationContext context)
        {
            var discriminators = context.Settings.Get<HashSet<string>>("ReceiverSideDistribution.Discriminators");
            var discriminator = context.Settings.Get<string>("EndpointInstanceDiscriminator");
            var transportInfrastructure = context.Settings.Get<TransportInfrastructure>();
            var localAddress = context.Settings.LogicalAddress();
            
            context.Pipeline.Register(new ReceiverSideDistributionBehavior(discriminator, discriminators, address => transportInfrastructure.ToTransportAddress(address), localAddress), "Distributes on the receiver side");
        }

        class ReceiverSideDistributionBehavior : IBehavior<IIncomingPhysicalMessageContext, IIncomingPhysicalMessageContext>
        {
            private HashSet<string> discriminators;
            private string discriminator;
            private Func<LogicalAddress, string> addressTranslator;
            private LogicalAddress logicalAddress;

            public ReceiverSideDistributionBehavior(string discriminator, HashSet<string> discriminators, Func<LogicalAddress, string> addressTranslator, LogicalAddress local)
            {
                logicalAddress = local;
                this.addressTranslator = addressTranslator;
                this.discriminator = discriminator;
                this.discriminators = discriminators;
            }

            public async Task Invoke(IIncomingPhysicalMessageContext context, Func<IIncomingPhysicalMessageContext, Task> next)
            {
                string partitionKey;
                if (context.MessageHeaders.TryGetValue(PartitionHeaders.PartitionKey, out partitionKey))
                {
                    if (partitionKey == discriminator)
                    {
                        await next(context).ConfigureAwait(false);
                        return;
                    }

                    if (discriminators.Contains(partitionKey))
                    {
                        var destination = addressTranslator(logicalAddress.CreateIndividualizedAddress(partitionKey));
                        await context.ForwardCurrentMessageTo(destination).ConfigureAwait(false);
                        return;
                    }
                    
                    // forward to error
                    return;
                }
                await next(context).ConfigureAwait(false);
            }
        }
    }
}
