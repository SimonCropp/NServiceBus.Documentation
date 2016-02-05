﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Pipeline;

class ForwardToSqlTransportBehavior : Behavior<ITransportReceiveContext>
{
    static ILog logger = LogManager.GetLogger<ForwardToSqlTransportBehavior>();

    public override async Task Invoke(ITransportReceiveContext context, Func<Task> next)
    {
        var message = context.Message;
        Type[] eventTypes =
        {
            Type.GetType(message.Headers["NServiceBus.EnclosedMessageTypes"])
        };

        string msmqId = message.Headers["NServiceBus.MessageId"];

        // Set the Id to a deterministic guid, as Sql message Ids are Guids and Msmq message ids are guid\nnnn.
        // Newer versions of Nsb already return just a guid for the messageId. So, check to see if the Id is a valid Guid and if 
        // not, only then create a valid Guid. This check is important as it affects the retries if the message is rolled back.
        // If the Ids are different, then the FLR/SLR won't know its the same message.
        Guid newGuid;
        if (!Guid.TryParse(msmqId, out newGuid))
        {
            message.Headers["NServiceBus.MessageId"] = GuidBuilder.BuildDeterministicGuid(msmqId).ToString();
        }
        logger.Info("Forwarding message to the SQL Relay");

        var connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=PersistenceForSqlTransport;Integrated Security=True";
        await SendMessageToSql(connectionString, "SqlRelay", message.Body, message.Headers);
    }

    public Task SendMessageToSql(string connectionString, string queue, byte[] messageBody, Dictionary<string, string> headers)
    {
        string insertSql = $@"INSERT INTO [{queue}] (
                                [Id], 
                                [Recoverable], 
                                [Headers], 
                                [Body])
                            VALUES (
                                @Id, 
                                @Recoverable, 
                                @Headers, 
                                @Body)";
        using (TransactionScope scope = new TransactionScope())
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(insertSql, connection))
                {
                    SqlParameterCollection parameters = command.Parameters;
                    command.CommandType = CommandType.Text;
                    parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                    string serializeHeaders = JsonConvert.SerializeObject(headers);
                    parameters.Add("Headers", SqlDbType.VarChar).Value = serializeHeaders;
                    parameters.Add("Body", SqlDbType.VarBinary).Value = messageBody;
                    parameters.Add("Recoverable", SqlDbType.Bit).Value = true;
                    command.ExecuteNonQuery();
                }
            }
            scope.Complete();
        }
        return Task.FromResult(0);
    }
}

class NewMessageProcessingPipelineStep : RegisterStep
{
    public NewMessageProcessingPipelineStep()
        : base("ForwardMessagesToSqlTransport", typeof(ForwardToSqlTransportBehavior), "Forwards MSMQ events received to SqlRelay")
    {
        InsertAfterIfExists("FirstLevelRetries");
        InsertAfterIfExists("SecondLevelRetries");
        InsertAfterIfExists("MoveFaultsToErrorQueue");
    }
}

class RegisterSqlRelay : INeedInitialization
{
    public void Customize(BusConfiguration configuration)
    {
        configuration.Pipeline.Register<NewMessageProcessingPipelineStep>();
    }
}