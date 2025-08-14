using System;

public abstract class State
{
	public abstract void OnEnter(params object[] parameters);
	public abstract void OnTick(params object[] parameters);
	public abstract void OnExit(params object[] parameters);
}

public sealed class PatrolState : State
{
	public override void OnEnter(params object[] parameters)
	{
	}

	public override void OnExit(params object[] parameters)
	{
	}

	public override void OnTick(params object[] parameters)
	{

	}
}

public sealed class ChaseState : State
{
	public override void OnEnter(params object[] parameters)
	{
	}

	public override void OnExit(params object[] parameters)
	{
	}

	public override void OnTick(params object[] parameters)
	{
		UnityEngine.Debug.Log("Chase");

	}
}

public sealed class AttackState : State
{
	public override void OnEnter(params object[] parameters)
	{
	}

	public override void OnExit(params object[] parameters)
	{
	}

	public override void OnTick(params object[] parameters)
	{
		UnityEngine.Debug.Log("Attack");
	}
}

public sealed class BackToPatrolState : State
{
	public override void OnEnter(params object[] parameters)
	{
	}

	public override void OnExit(params object[] parameters)
	{
	}

	public override void OnTick(params object[] parameters)
	{
		UnityEngine.Debug.Log("BackToPatrol");
	}
}