﻿namespace Snippets5.Transports.SqlServer_2
{
    using NServiceBus;
    using NServiceBus.Transports.SQLServer;

    class MultiSchema
    {
        void NonStandardSchema()
        {
            BusConfiguration busConfiguration = new BusConfiguration();

            #region sqlserver-non-standard-schema 2.1

            busConfiguration.UseTransport<SqlServerTransport>()
                .DefaultSchema("myschema");

            #endregion
        }

        void NonStandardSchemaInConnectionString()
        {
            BusConfiguration busConfiguration = new BusConfiguration();

            #region sqlserver-non-standard-schema-connString [2.1,2.0]

            busConfiguration.UseTransport<SqlServerTransport>()
                .ConnectionString("Data Source=INSTANCE_NAME;Initial Catalog=some_database;Integrated Security=True; Queue Schema=myschema");

            #endregion
        }

        void OtherEndpointConnectionParamsPush()
        {
            BusConfiguration busConfiguration = new BusConfiguration();

            #region sqlserver-multischema-config-push [2.1,2.0]

            busConfiguration.UseTransport<SqlServerTransport>()
                .UseSpecificConnectionInformation(
                    EndpointConnectionInfo.For("sales")
                        .UseSchema("sender"),
                    EndpointConnectionInfo.For("billing")
                        .UseSchema("receiver")
                );

            #endregion
        }

        void OtherEndpointConnectionParamsPull()
        {
            BusConfiguration busConfiguration = new BusConfiguration();
            #region sqlserver-multischema-config-pull 2.1

            busConfiguration.UseTransport<SqlServerTransport>()
                .UseSpecificConnectionInformation(endpointName =>
                {
                    if (endpointName == "sales")
                        return ConnectionInfo.Create()
                                             .UseSchema("salesSchema");
                    if (endpointName == "billing")
                        return ConnectionInfo.Create()
                                             .UseSchema("billingSchema");
                    return null;
                });

            #endregion
        }
    }
}
