using System.Threading.Tasks;
using NServiceBus;

static class MySessionContextExtensions
{
    public static IMySession GetSession(this IMessageHandlerContext context)
    {
        return context.Extensions.Get<IMySession>();
    }

    public static Task Store<T>(this IMessageHandlerContext context, T entity)
    {
        return context.Extensions.Get<IMySession>().Store(entity);
    }
}