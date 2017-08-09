﻿namespace SqsAll.QueueCreation
{
    using System;
    using System.Threading.Tasks;
    using Amazon.SQS;
    using Amazon.SQS.Model;

    #region sqs-create-queues

    public static class QueueCreationUtils
    {
        static TimeSpan DefaultTimeToLive = TimeSpan.FromDays(4);

        public static async Task CreateQueue(string queueName, TimeSpan? maxTimeToLive = null, string queueNamePrefix = null, bool preTruncateQueueNames = false)
        {
            try
            {
                using (var client = ClientFactory.CreateSqsClient())
                {
                    var sqsRequest = new CreateQueueRequest
                    {
                        QueueName = QueueNameHelper.GetSqsQueueName(queueName, queueNamePrefix, preTruncateQueueNames)
                    };
                    var value = Convert.ToInt32((maxTimeToLive ?? DefaultTimeToLive).TotalSeconds).ToString();
                    sqsRequest.Attributes.Add(QueueAttributeName.MessageRetentionPeriod, value);
                    await client.CreateQueueAsync(sqsRequest)
                        .ConfigureAwait(false);
                }
            }
            catch (QueueNameExistsException)
            {
            }
        }
    }

    #endregion
}