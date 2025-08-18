﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public struct BehaviourActions
{
    private Dictionary<int, List<Action>> mainThreadBehaviours;
    private ConcurrentDictionary<int, List<Action>> multiThreadableBehaviours;
    private Action transitionBehaviour;

    public Dictionary<int, List<Action>> MainThreadBehaviours => mainThreadBehaviours;
    public ConcurrentDictionary<int, List<Action>> MultiThreadableBehaviours => multiThreadableBehaviours;
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
            multiThreadableBehaviours = new ConcurrentDictionary<int, List<Action>>();
        if (!multiThreadableBehaviours.ContainsKey(exceutionOrder))
            multiThreadableBehaviours.TryAdd(exceutionOrder, new List<Action>());

        multiThreadableBehaviours[exceutionOrder].Add(behaviour);
    }

    public void SetTransitionBehaviour(Action behaviour)
    {
        transitionBehaviour = behaviour;
    }
}

public abstract class State
{
    public Action<Enum> OnFlag;
    public abstract BehaviourActions GetOnEnterBehaviours(params object[] parameters);
    public abstract BehaviourActions GetOnTickBehaviours(params object[] parameters);
    public abstract BehaviourActions GetOnExitBehaviour(params object[] parameters);
}

public sealed class PatrolState : State
{
    private Transform actualTarget;

    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehaviourActions GetOnExitBehaviour(params object[] parameters)
    {
        return default;
    }

    public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
    {
        Transform wayPoint1 = parameters[0] as Transform;
        Transform wayPoint2 = parameters[1] as Transform;
        Transform agentTransform = parameters[2] as Transform;
        Transform targetTransform = parameters[3] as Transform;
        float speed = (float)parameters[4];
        float chaseDistance = (float)parameters[5];
        float deltaTime = (float)parameters[6];

        BehaviourActions behaviourActions = new BehaviourActions();

        behaviourActions.AddMainTrheadableBehaviour(0, () =>
        {
            if (actualTarget == null)
            {
                actualTarget = wayPoint1;
            }

            if (Vector3.Distance(agentTransform.position, actualTarget.position) < 0.1f)
            {
                if (actualTarget == wayPoint1)
                {
                    actualTarget = wayPoint2;
                }
                else
                {
                    actualTarget = wayPoint1;
                }
            }
        });

        behaviourActions.AddMainTrheadableBehaviour(1, () =>
        {
            agentTransform.position += (actualTarget.position - agentTransform.position).normalized * speed * deltaTime;
        });

        behaviourActions.SetTransitionBehaviour(() =>
        {
            if (Vector3.Distance(agentTransform.position, targetTransform.position) <= chaseDistance)
            {
                OnFlag?.Invoke(Agent.Flags.OnTargetNear);
            }
        });
        return behaviourActions;


    }
}

public sealed class ChaseState : State
{
    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehaviourActions GetOnExitBehaviour(params object[] parameters)
    {
        return default;
    }

    public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
    {
        Transform agentTrnasform = parameters[0] as Transform;
        Transform targetTransform = parameters[1] as Transform;
        float speed = (float)parameters[2];
        float explodeDistance = (float)parameters[3];
        float lostDistance = (float)parameters[4];
        float deltaTime = (float)parameters[5];

        BehaviourActions behaviourActions = new BehaviourActions();

        behaviourActions.AddMainTrheadableBehaviour(0, () =>
        {
            agentTrnasform.position += (targetTransform.position - agentTrnasform.position).normalized * speed * deltaTime;
        });

        behaviourActions.SetTransitionBehaviour(() =>
        {
            if (Vector3.Distance(agentTrnasform.position, targetTransform.position) < explodeDistance)
            {
                OnFlag.Invoke(Agent.Flags.OnTargetReach);
            }

            if (Vector3.Distance(agentTrnasform.position, targetTransform.position) > lostDistance)
            {
                OnFlag.Invoke(Agent.Flags.OnTargetLost);
            }
        });

        return behaviourActions;
    }
}

public sealed class ExplodeState : State
{
    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehaviourActions behaviourActions = new BehaviourActions();
        behaviourActions.AddMultiTrheadableBehaviour(0, () => { Debug.Log("Boom"); });
        return behaviourActions;
    }

    public override BehaviourActions GetOnExitBehaviour(params object[] parameters)
    {
        return default;
    }

    public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
    {
        BehaviourActions behaviourActions = new BehaviourActions();
        behaviourActions.AddMultiTrheadableBehaviour(0, () => { Debug.Log("F"); });
        return behaviourActions;
    }
}

