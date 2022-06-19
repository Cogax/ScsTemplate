using System.Collections.Concurrent;

namespace Poc.Nsb.Outbox.Worker;

public class Store
{
    public ConcurrentStack<object> Stack = new ConcurrentStack<object>();
}
