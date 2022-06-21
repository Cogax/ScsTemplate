using System.Collections.Concurrent;

namespace Cogax.SelfContainedSystem.Template.Worker;

public class Store
{
    public ConcurrentStack<object> Stack = new ConcurrentStack<object>();
}
