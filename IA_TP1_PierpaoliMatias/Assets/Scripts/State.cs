using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;
using UnityEngine;

public struct BehaviourActions
{
	private Dictionary<int, List<Action>> mainThreadBehaviours;
	private ConcurrentDictionary<int, List<Action>> multithredableBehaviours;
	private Action transitionBehavior;

	public Dictionary<int, List<Action>> MainThreadBehaviours => mainThreadBehaviours;
	public ConcurrentDictionary<int, List<Action>> MultiThreadBehaviours => multithredableBehaviours;
	public Action TransitionBehavior => transitionBehavior;

	public void AddMainTrheadableBehaviour(int executionOrder, Action behaviour)
	{
		if (mainThreadBehaviours == null)
			mainThreadBehaviours = new Dictionary<int, List<Action>>();
		if (mainThreadBehaviours.ContainsKey(executionOrder))
			mainThreadBehaviours.Add(executionOrder, new List<Action>());

        mainThreadBehaviours[executionOrder].Add(behaviour);
	}

    public void AddMultiTrheadableBehaviour(int executionOrder, Action behaviour)
    {
        if (multithredableBehaviours == null)
            multithredableBehaviours = new ConcurrentDictionary<int, List<Action>>();
        if (multithredableBehaviours.ContainsKey(executionOrder))
            multithredableBehaviours.TryAdd(executionOrder, new List<Action>());

        multithredableBehaviours[executionOrder].Add(behaviour);
    }

	public void SetTransitionBehaviour(Action behaviour)
	{
		transitionBehavior = behaviour;
	}





}

public abstract class State
{
	public Action<Enum> OnFlag;
	public abstract BehaviourActions GetOnEnterBehaviours(params object[] parameters);
	public abstract BehaviourActions GetOnExitBehaviours(params object[] parameters);
	public abstract BehaviourActions GetOnTickBehaviours(params object[] parameters);
}

public sealed class PatrolState : State
{
	public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
	{
		return default;
	}

	public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
	{
        return default;
    }

	public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
	{
        //      Transform myPoint1 = parameters[0] as Transform;
        //      Transform myPoint2 = parameters[1] as Transform;
        //      Transform agentTransform = parameters[2] as Transform;
        //      Transform targetTransform = parameters[3] as Transform;
        //      float speed = (float)parameters[4];
        //      float chasedDistance = (float)parameters[5];
        //      float deltaTime = (float)parameters[6];

        //BehaviourActions behaviourActions = new BehaviourActions();

        //behaviourActions.AddMainTrheadableBehaviour(0, () =>
        //{
        //	if (actualTarget = null)
        //	{
        //		actualTarget = myPoint1;
        //          }

        //	if (Vector3.Distance(agentTransform.position, actualTarget.position) < 0.1f)
        //	{
        //              if (actualTarget = myPoint1)
        //              {
        //                  actualTarget = myPoint2;
        //              }
        //		else 
        //		{
        //			actualTarget = myPoint1;
        //              }
        //          }
        //});

        //behaviourActions.AddMainTrheadableBehaviour(1, () =>
        //{
        //	agentTransform.position += (actualTarget.position - agentTransform.position).normalized * speed * deltaTime;
        //});

        //      behaviourActions.SetTransitionBehaviour(() =>
        //      {
        //          if (Vector3.Distance(agentTransform.position, targetTransform.position) <= chasedDistance)
        //	{
        //		OnFlag?.Invoke(Agent.Flags.OnTargetNear);
        //	}
        //      });

        //      return behaviourActions;

        return default;
    }
}

public sealed class ChaseState : State
{
	public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        return default;
    }

	public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

	public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
    {
		Transform agentTransform = parameters[0] as Transform;
		Transform targetTransform = parameters[1] as Transform;
		float speed = (float)parameters[2];
		float explodedDistance = (float)parameters[3];
		float lostDistance = (float)parameters[4];
		float deltaTime = (float)parameters[5];

		BehaviourActions behaviourActions = new BehaviourActions();

		behaviourActions.AddMainTrheadableBehaviour(0, () =>
		{
			agentTransform.position = (targetTransform.position - agentTransform.position).normalized * speed * deltaTime;
		});

		behaviourActions.SetTransitionBehaviour(() =>
		{
			if (Vector3.Distance(agentTransform.position, targetTransform.position) < explodedDistance)
			{
				OnFlag?.Invoke(Agent.Flags.OnTargetReach);
			}

            if (Vector3.Distance(agentTransform.position, targetTransform.position) > explodedDistance)
            {
                OnFlag?.Invoke(Agent.Flags.OnTargetLost);
            }

        });

        return behaviourActions;
    }
}

public sealed class AttackState : State
{
	public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        return default;
    }

	public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

	public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
	{
		UnityEngine.Debug.Log("Attack"); 
		return default;
    }
}

public sealed class BackToPatrolState : State
{
	public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        return default;
    }

	public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

	public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
	{
		UnityEngine.Debug.Log("BackToPatrol");
        return default;
    }
}

public sealed class ExplodedState : State
{
    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
    {
        return default;
    }
}

