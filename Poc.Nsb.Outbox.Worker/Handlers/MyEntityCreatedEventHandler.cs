using NServiceBus;

namespace Poc.Nsb.Outbox.Worker.Handlers;

//public class MyEntityCreatedEventHandler : IHandleMessages<MyEntityCreatedEvent>
//{
//    private readonly Store _store;

//    public MyEntityCreatedEventHandler(Store store)
//    {
//        _store = store;
//    }

//    public Task Handle(MyEntityCreatedEvent message, IMessageHandlerContext context)
//    {
//        _store.Stack.Push(message);
//        return Task.CompletedTask;
//    }
//}
