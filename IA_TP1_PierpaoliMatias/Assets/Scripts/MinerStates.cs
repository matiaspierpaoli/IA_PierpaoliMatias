using System;
using UnityEngine;

public sealed class GoToMineState : State
{
    public override Type[] OnTickParameterTypes => new[] { typeof(MinerContext), typeof(float) };
    
    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        var behavior = ConcurrentPool.Get<BehaviourActions>();
        behavior.AddMainTrheadableBehaviour(0, () => Debug.Log("Yendo a la mina pa"));
        return behavior;
    }

    public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
    {
        var ctx = (MinerContext)parameters[0];
        float deltaTime = (float)parameters[1];

        var behavior = ConcurrentPool.Get<BehaviourActions>();
        behavior.AddMainTrheadableBehaviour(0, () =>
        {
            Vector3 to = ctx.mine.position - ctx.agent.position;
            if (to.sqrMagnitude > ctx.arrivalRadius * ctx.arrivalRadius)
                ctx.agent.position += to.normalized * ctx.speed * deltaTime;
        });
        behavior.SetTransitionBehaviour(() =>
        {
            if (Vector3.Distance(ctx.agent.position, ctx.mine.position) <= ctx.arrivalRadius)
                OnFlag?.Invoke(Agent.Flags.ArrivedMine);
        });
        return behavior;
    }

    public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
    {
        var behavior = ConcurrentPool.Get<BehaviourActions>();
        behavior.AddMainTrheadableBehaviour(0, () => Debug.Log("Llegue a la mina"));
        return behavior;
    }
}

public sealed class MineState : State
{
    public override Type[] OnTickParameterTypes => new[] { typeof(MinerContext), typeof(float) };

    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        var behavior = ConcurrentPool.Get<BehaviourActions>();
        behavior.AddMainTrheadableBehaviour(0, () => Debug.Log("Minando"));
        return behavior;
    }

    public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
    {
        var ctx = (MinerContext)parameters[0];
        float deltaTime = (float)parameters[1];

        var behavior = ConcurrentPool.Get<BehaviourActions>();
        behavior.AddMainTrheadableBehaviour(0, () =>
        {
            if (ctx.mineRemaining <= 0) return;

            ctx.gatherAccumulator += ctx.gatherRate * deltaTime;

            while (ctx.gatherAccumulator >= 1f && ctx.inventory < ctx.capacity && ctx.mineRemaining > 0)
            {
                ctx.inventory++;
                ctx.mineRemaining--;
                ctx.gatherAccumulator -= 1f;

                Debug.Log($"<color=#4CAF50>[MINING] inv={ctx.inventory}/{ctx.capacity} | mina={ctx.mineRemaining}");
            }
        });

        behavior.SetTransitionBehaviour(() =>
        {
            if (ctx.inventory >= ctx.capacity)
            {
                Debug.Log("Inventario lleno");
                OnFlag?.Invoke(Agent.Flags.InventoryFull);
            }
            else if (ctx.mineRemaining <= 0f)
            {
                Debug.Log("Mina vaciada");
                OnFlag?.Invoke(Agent.Flags.MineDepleted);
            }
        });
        return behavior;
    }

    public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
    {
        var behavior = ConcurrentPool.Get<BehaviourActions>();
        behavior.AddMainTrheadableBehaviour(0, () => Debug.Log("Termine de minar"));
        return behavior;
    }
}

public sealed class ReturnToBaseState : State
{
    public override Type[] OnTickParameterTypes => new[] { typeof(MinerContext), typeof(float) };

    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        var behavior = ConcurrentPool.Get<BehaviourActions>();
        behavior.AddMainTrheadableBehaviour(0, () => Debug.Log("Dictador a la base"));
        return behavior;
    }

    public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
    {
        var ctx = (MinerContext)parameters[0];
        float deltaTime = (float)parameters[1];

        var behavior = ConcurrentPool.Get<BehaviourActions>();
        behavior.AddMainTrheadableBehaviour(0, () =>
        {
            Vector3 to = ctx.home.position - ctx.agent.position;
            if (to.sqrMagnitude > ctx.arrivalRadius * ctx.arrivalRadius)
                ctx.agent.position += to.normalized * ctx.speed * deltaTime;
        });
        behavior.SetTransitionBehaviour(() =>
        {
            if (Vector3.Distance(ctx.agent.position, ctx.home.position) <= ctx.arrivalRadius)
                OnFlag?.Invoke(Agent.Flags.ArrivedBase);
        });
        return behavior;
    }

    public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
    {
        var behavior = ConcurrentPool.Get<BehaviourActions>();
        behavior.AddMainTrheadableBehaviour(0, () => Debug.Log("Llegue a la base"));
        return behavior;
    }
}

public sealed class DepositState : State
{
    public override Type[] OnTickParameterTypes => new[] { typeof(MinerContext), typeof(float) };

    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        var behavior = ConcurrentPool.Get<BehaviourActions>();
        behavior.AddMainTrheadableBehaviour(0, () => Debug.Log("Depositando"));
        return behavior;
    }

    public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
    {
        var ctx = (MinerContext)parameters[0];
        float dt = (float)parameters[1];

        var behavior = ConcurrentPool.Get<BehaviourActions>();
        behavior.AddMainTrheadableBehaviour(0, () =>
        {
            if (ctx.inventory > 0)
            {
                ctx.depositAccumulator += ctx.depositRate * dt;

                while (ctx.depositAccumulator >= 1f && ctx.inventory > 0)
                {
                    ctx.inventory--;
                    ctx.depositAccumulator -= 1f;
                    Debug.Log($"\"<color=#03A9F4>[DEPOSIT] inv={ctx.inventory}/{ctx.capacity}");
                }
            }
        });

        behavior.SetTransitionBehaviour(() =>
        {
            if (ctx.inventory == 0)
            {
                if (ctx.mineRemaining > 0f)
                {
                    Debug.Log("Necesito seguir minando");
                    OnFlag?.Invoke(Agent.Flags.NeedToMine);
                }
                else
                {
                    OnFlag?.Invoke(Agent.Flags.WorkDone);
                }
            }
        });

        return behavior;
    }
}

public sealed class DoneState : State
{
    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        var behavior = ConcurrentPool.Get<BehaviourActions>();
        behavior.AddMainTrheadableBehaviour(0, () => Debug.Log("Trabajo terminado."));
        return behavior;
    }
}