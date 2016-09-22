﻿namespace Testing_6.ServiceLayer
{
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Testing;
    using NUnit.Framework;

    [Explicit]
    #region TestingServiceLayer
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestHandler()
        {
            Test.Handler<MyHandler>()
                .ExpectReply<ResponseMessage>(m => m.String == "hello")
                .OnMessage<RequestMessage>(m => m.String = "hello");
        }
    }

    public class MyHandler :
        IHandleMessages<RequestMessage>
    {
        public Task Handle(RequestMessage message, IMessageHandlerContext context)
        {
            var reply = new ResponseMessage
            {
                String = message.String
            };
            return context.Reply(reply);
        }
    }

    #endregion
}