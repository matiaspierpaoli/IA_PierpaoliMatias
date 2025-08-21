using System;
using UnityEngine;

public sealed class ChaseState : State
{
    public override Type[] OnTickParameterTypes => new Type[]
    {
        typeof(Transform), // agentTransform
        typeof(Transform), // targetTransform
        typeof(float), // speed
        typeof(float), // explodeDistance
        typeof(float), // lostDistance
        typeof(float) // deltTime
    };

    public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
    {
        Transform agentTrnasform = parameters[0] as Transform;
        Transform targetTransform = parameters[1] as Transform;
        float speed = (float)parameters[2];
        float explodeDistance = (float)parameters[3];
        float lostDistance = (float)parameters[4];
        float deltaTime = (float)parameters[5];

        BehaviourActions behaviourActions = ConcurrentPool.Get<BehaviourActions>();

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

