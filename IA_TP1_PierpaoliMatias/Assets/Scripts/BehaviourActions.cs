using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class BehaviourActions : IReseatable
{
    private Dictionary<int, List<Action>> mainThreadBehaviours;
    private ConcurrentDictionary<int, ConcurrentBag<Action>> multiThreadableBehaviours;
    private Action transitionBehaviour;

    public Dictionary<int, List<Action>> MainThreadBehaviours => mainThreadBehaviours;
    public ConcurrentDictionary<int, ConcurrentBag<Action>> MultiThreadableBehaviours => multiThreadableBehaviours;
    public Action TransitionBehaviour => transitionBehaviour;

    public void AddMainTrheadableBehaviour(int exceutionOrder, Action behaviour)
    {
        if (mainThreadBehaviours == null)
            mainThreadBehaviours = new Dictionary<int, List<Action>>();
        if (!mainThreadBehaviours.ContainsKey(exceutionOrder))
            mainThreadBehaviours.Add(exceutionOrder, new List<Action>());

        mainThreadBehaviours[exceutionOrder].Add(behaviour);
    }

    public void AddMultiTrheadableBehaviour(int exceutionOrder, Action behaviour)
    {
        if (multiThreadableBehaviours == null)
            multiThreadableBehaviours = new ConcurrentDictionary<int, ConcurrentBag<Action>>();
        if (!multiThreadableBehaviours.ContainsKey(exceutionOrder))
            multiThreadableBehaviours.TryAdd(exceutionOrder, new ConcurrentBag<Action>());

        multiThreadableBehaviours[exceutionOrder].Add(behaviour);
    }


    public void SetTransitionBehaviour(Action behaviour)
    {
        transitionBehaviour = behaviour;
    }

    public void Reset()
    {
        if (mainThreadBehaviours != null)
        {
            foreach (KeyValuePair<int, List<Action>> behaviour in mainThreadBehaviours)
            {
                behaviour.Value.Clear();
            }
            mainThreadBehaviours.Clear();
        }
        if (multiThreadableBehaviours != null)
        {
            foreach (KeyValuePair<int, ConcurrentBag<Action>> behaviour in multiThreadableBehaviours)
            {
                behaviour.Value.Clear();
            }
            multiThreadableBehaviours.Clear();
        }
        transitionBehaviour = null;
    }
}

