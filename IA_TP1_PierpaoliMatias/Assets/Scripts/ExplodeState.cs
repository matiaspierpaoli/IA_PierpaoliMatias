using UnityEngine;

public sealed class ExplodeState : State
{
    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehaviourActions behaviourActions = ConcurrentPool.Get<BehaviourActions>();
        behaviourActions.AddMultiTrheadableBehaviour(0, () => { Debug.Log("Boom"); });
        return behaviourActions;
    }

    public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
    {
        BehaviourActions behaviourActions = ConcurrentPool.Get<BehaviourActions>();
        behaviourActions.AddMultiTrheadableBehaviour(0, () => { Debug.Log("F"); });
        return behaviourActions;
    }
}

