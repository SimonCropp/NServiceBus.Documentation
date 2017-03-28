﻿namespace Core6.ImmediateDispatch
{
    using System.Threading.Tasks;
    using System.Transactions;
    using NServiceBus;

    class Usage
    {
        async Task RequestImmediateDispatch(IPipelineContext context)
        {
            #region RequestImmediateDispatch
            var options = new SendOptions();
            options.RequireImmediateDispatch();
            var message = new MyMessage();
            await context.Send(message, options)
                .ConfigureAwait(false);
            #endregion
        }

        class MyMessage
        {
        }
    }
}
