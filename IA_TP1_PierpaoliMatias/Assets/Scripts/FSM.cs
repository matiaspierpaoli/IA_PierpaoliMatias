using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FSM<StateType, FlagType>
    where StateType : Enum
    where FlagType : Enum
{
    private const int UNNASSIGNED_TRANSITION = -1;
    private int currentState;
    private Dictionary<int, State> states;
    private Dictionary<int, Func<object[]>> behaviourOnTickParameters;
    private Dictionary<int, Func<object[]>> behaviourOnEnterParameters;
    private Dictionary<int, Func<object[]>> behaviourOnExitParameters;

    private (int destinationState, Action onTrnasition)[,] transitions;

    ParallelOptions ParallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 32 };

    private BehaviourActions GetCurrentTickBehaviour => states[currentState].GetOnTickBehaviours
        (behaviourOnTickParameters[currentState]?.Invoke());
    private BehaviourActions GetCurrentOnEnterBehaviour => states[currentState].GetOnEnterBehaviours
        (behaviourOnEnterParameters[currentState]?.Invoke());
    private BehaviourActions GetCurrentOnExitBehaviour => states[currentState].GetOnExitBehaviour
        (behaviourOnExitParameters[currentState]?.Invoke());

    public FSM(StateType defaultState)
    {
        states = new Dictionary<int, State>();
        transitions = new (int, Action)[Enum.GetValues(typeof(StateType)).Length, Enum.GetValues(typeof(FlagType)).Length];
        for (int i = 0; i < transitions.GetLength(0); i++)
        {
            for (int j = 0; j < transitions.GetLength(1); j++)
            {
                transitions[i, j] = (UNNASSIGNED_TRANSITION, null);
            }
        }

        behaviourOnTickParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnEnterParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnExitParameters = new Dictionary<int, Func<object[]>>();
        ForceState(defaultState);
    }

    public void AddState<TState>(StateType stateIndex,
        Func<object[]> onTickParameters = null,
        Func<object[]> onEnterParameters = null,
        Func<object[]> onExitParameters = null)
        where TState : State, new()
    {
        if (!states.ContainsKey(Convert.ToInt32(stateIndex)))
        {
            TState state = new TState();
            state.OnFlag += Transition;
            states.Add(Convert.ToInt32(stateIndex), state);
            behaviourOnTickParameters.Add(Convert.ToInt32(stateIndex), onTickParameters);
            behaviourOnEnterParameters.Add(Convert.ToInt32(stateIndex), onEnterParameters);
            behaviourOnExitParameters.Add(Convert.ToInt32(stateIndex), onExitParameters);
        }
    }

    private void ForceState(StateType state)
    {
        currentState = Convert.ToInt32(state);
    }

    public void SetTransition(StateType originalState, FlagType flag, StateType destinationState, Action onTransition = null)
    {
        transitions[Convert.ToInt32(originalState), Convert.ToInt32(flag)] = (Convert.ToInt32(destinationState), onTransition);
    }

    public void Transition(Enum flag)
    {
        if (states.ContainsKey(currentState))
        {
            ExcecuteBehaviour(GetCurrentOnExitBehaviour);
        }
        if (transitions[Convert.ToInt32(currentState), Convert.ToInt32(flag)].destinationState != UNNASSIGNED_TRANSITION)
        {
            transitions[currentState, Convert.ToInt32(flag)].onTrnasition?.Invoke();
            currentState = transitions[Convert.ToInt32(currentState), Convert.ToInt32(flag)].destinationState;
            ExcecuteBehaviour(GetCurrentOnEnterBehaviour);
        }
    }

    public void Tick()
    {
        if (states.ContainsKey(currentState))
        {
            ExcecuteBehaviour(GetCurrentTickBehaviour);
        }
    }

    private void ExcecuteBehaviour(BehaviourActions behaviourActions)
    {
        if (behaviourActions.Equals(default))
            return;

        int excecutionOrder = 0;

        while ((behaviourActions.MainThreadBehaviours != null && behaviourActions.MainThreadBehaviours.Count > 0) ||
                behaviourActions.MultiThreadableBehaviours != null && behaviourActions.MultiThreadableBehaviours.Count > 0)
        {
            Task multithradableBehabiour = new Task(() =>
            {
                if (behaviourActions.MultiThreadableBehaviours != null)
                {
                    if (behaviourActions.MultiThreadableBehaviours.ContainsKey(excecutionOrder))
                    {
                        Parallel.ForEach(behaviourActions.MultiThreadableBehaviours[excecutionOrder], ParallelOptions, (behaviour) =>
                        {
                            behaviour?.Invoke();
                        });
                        behaviourActions.MultiThreadableBehaviours.TryRemove(excecutionOrder, out _);
                    }
                }
            });

            multithradableBehabiour.Start();

            if (behaviourActions.MainThreadBehaviours != null)
            {
                if (behaviourActions.MainThreadBehaviours.ContainsKey(excecutionOrder))
                {
                    foreach (Action behaviour in behaviourActions.MainThreadBehaviours[excecutionOrder])
                    {
                        behaviour?.Invoke();
                    }
                    behaviourActions.MainThreadBehaviours.Remove(excecutionOrder);
                }
            }

            multithradableBehabiour?.Wait();
            excecutionOrder++;
        }

        behaviourActions.TransitionBehaviour?.Invoke();
    }
}