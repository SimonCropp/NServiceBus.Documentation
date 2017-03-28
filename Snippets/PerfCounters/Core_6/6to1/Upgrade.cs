﻿namespace Core6
{
    using System;
    using NServiceBus;

    class Upgreade
    {

        void EnablingCriticalTime(EndpointConfiguration endpointConfiguration)
        {
            #region 6to1-enable-criticaltime

            endpointConfiguration.EnableCriticalTimePerformanceCounter();

            #endregion
        }
        void EnablingSla(EndpointConfiguration endpointConfiguration)
        {
            #region 6to1-enable-sla

            endpointConfiguration.EnableSLAPerformanceCounter(TimeSpan.FromMinutes(3));

            #endregion
        }


    }

}