using System;
using UnityEngine;

public sealed class PatrolState : State
{
    private Transform actualTarget;

    public override Type[] OnTickParameterTypes => new Type[]
    {
        typeof(Transform), // waypoint1
        typeof(Transform), // waypoint2
        typeof(Transform), // agentTransform
        typeof(Transform), // targetTransform
        typeof(float), // speed
        typeof(float), // chaseDistance
        typeof(float) // deltaTime
    };

    public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
    {
        Transform wayPoint1 = parameters[0] as Transform;
        Transform wayPoint2 = parameters[1] as Transform;
        Transform agentTransform = parameters[2] as Transform;
        Transform targetTransform = parameters[3] as Transform;
        float speed = (float)parameters[4];
        float chaseDistance = (float)parameters[5];
        float deltaTime = (float)parameters[6];

        BehaviourActions behaviourActions = ConcurrentPool.Get<BehaviourActions>();

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

