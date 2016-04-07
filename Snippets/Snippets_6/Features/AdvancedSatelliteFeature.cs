﻿namespace Snippets6.Features
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Features;
    using NServiceBus.Pipeline;
    using NServiceBus.Transports;

    public class MyAdvancedSatelliteFeature : Feature
    {
        public MyAdvancedSatelliteFeature()
        {
            EnableByDefault();
        }

        #region AdvancedSatelliteFeatureSetup
        protected override void Setup(FeatureConfigurationContext context)
        {
            PipelineSettings messageProcessorPipeline = context.AddSatellitePipeline("AdvancedSatellite", TransportTransactionMode.TransactionScope, PushRuntimeSettings.Default, "targetQueue");
            
            // register the critical error
            messageProcessorPipeline.Register("AdvancedSatellite", b => new MyAdvancedSatelliteBehavior(b.Build<CriticalError>()),
                    "Description of what the advanced satellite does");

        }
        #endregion
    }

    #region AdvancedSatelliteBehavior
    class MyAdvancedSatelliteBehavior : PipelineTerminator<ISatelliteProcessingContext>
    {
        CriticalError CriticalError { get; set; }
        public MyAdvancedSatelliteBehavior(CriticalError CriticalError)
        {
            this.CriticalError = CriticalError;
        }

        protected override Task Terminate(ISatelliteProcessingContext context)
        {
            // To raise a critical error
            CriticalError.Raise("Something bad happened - trigger critical error", new Exception("CriticalError occured!!"));
            return Task.FromResult(true);
        }
    }
    #endregion
}
