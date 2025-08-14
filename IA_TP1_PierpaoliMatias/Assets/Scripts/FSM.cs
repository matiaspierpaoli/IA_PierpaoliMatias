using System;
using System.Collections.Generic;

public class FSM<StateType, FlagType>
	where StateType : Enum
	where FlagType : Enum
{
	private const int UNNASSIGNED_TRANSITION = -1;
	private int currentState;
	private Dictionary<int, State> behabiours;
	private int[,] transitions;

	public FSM(StateType defaultState)
	{
		behabiours = new Dictionary<int, State>();
		transitions = new int[Enum.GetValues(typeof(StateType)).Length, Enum.GetValues(typeof(FlagType)).Length];
		for (int i = 0; i < transitions.GetLength(0); i++)
		{
			for (int j = 0; j < transitions.GetLength(1); j++)
			{
				transitions[i, j] = UNNASSIGNED_TRANSITION;
			}
		}
		ForceState(defaultState);
	}

	public void AddState<TState>(StateType stateIndex) where TState : State, new()
	{
		if (!behabiours.ContainsKey(Convert.ToInt32(stateIndex)))
		{
			TState state = new TState();
			behabiours.Add(Convert.ToInt32(stateIndex), state);
		}
	}

	private void ForceState(StateType state)
	{
		currentState = Convert.ToInt32(state);
	}

	public void SetTransition(StateType originalState, FlagType flag, StateType destinationState)
	{
		transitions[Convert.ToInt32(originalState), Convert.ToInt32(flag)] = Convert.ToInt32(destinationState);
	}

	public void Transition(FlagType flag)
	{
		if (behabiours.ContainsKey(currentState))
		{
			behabiours[currentState].OnExit();
		}
		if (transitions[Convert.ToInt32(currentState), Convert.ToInt32(flag)] != UNNASSIGNED_TRANSITION)
		{
			currentState = transitions[Convert.ToInt32(currentState), Convert.ToInt32(flag)];
			behabiours[currentState].OnEnter();
		}
	}

	public void Tick()
	{
		if (behabiours.ContainsKey(currentState))
		{
			behabiours[currentState].OnTick();
		}
	}
}