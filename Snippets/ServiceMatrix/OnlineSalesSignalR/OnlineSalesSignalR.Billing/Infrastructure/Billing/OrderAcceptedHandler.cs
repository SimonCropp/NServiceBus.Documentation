﻿//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by ServiceMatrix.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using NServiceBus;
using OnlineSalesSignalR.Contracts.Sales;


namespace OnlineSalesSignalR.Billing
{
    public partial class OrderAcceptedHandler : IHandleMessages<OrderAccepted>
    {
		
		public void Handle(OrderAccepted message)
		{
			// Handle message on partial class
			this.HandleImplementation(message);
		}

		partial void HandleImplementation(OrderAccepted message);

        public IBus Bus { get; set; }

    }

	
}
