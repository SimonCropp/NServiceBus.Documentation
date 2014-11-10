﻿//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by ServiceMatrix.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;

namespace OnlineSales.ECommerce
{
    public class MessageConventions : INeedInitialization
    {
        public void Customize(BusConfiguration config)
        {
            config.Conventions()
                .DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("OnlineSales.Internal.Commands"))
                .DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("OnlineSales.Contracts"))
                .DefiningMessagesAs(t => t.Namespace != null && t.Namespace.StartsWith("OnlineSales.Internal.Messages"));
        }
    }
}

