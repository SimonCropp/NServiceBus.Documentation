﻿namespace Core6.Recoverability.ErrorHandling
{
    using NServiceBus;

    class Usage
    {
        Usage(EndpointConfiguration endpointConfiguration)
        {
            #region ErrorWithCode

            endpointConfiguration.SendFailedMessagesTo("targetErrorQueue");

            #endregion
        }
    }
}