using System;
using System.Collections.Concurrent;

public class ConcurrentPool
{
    private static readonly ConcurrentDictionary<Type, ConcurrentStack<IReseatable>> concurrentPool =
        new ConcurrentDictionary<Type, ConcurrentStack<IReseatable>>();

    public static TReseatable Get<TReseatable>() where TReseatable : IReseatable, new()
    {
        if (!concurrentPool.ContainsKey(typeof(TReseatable)))
            concurrentPool.TryAdd(typeof(TReseatable), new ConcurrentStack<IReseatable>());

        TReseatable value;
        if (concurrentPool[typeof(TReseatable)].Count > 0)
        {
            concurrentPool[typeof(TReseatable)].TryPop(out IReseatable reseatable);
            value = (TReseatable)reseatable;
        }
        else
        {
            value = new TReseatable();
        }
        return value;
    }

    public static void Release<TReseatable>(TReseatable obj) where TReseatable : IReseatable, new()
    {
        obj.Reset();
        concurrentPool[typeof(TReseatable)].Push(obj);
    }
}
