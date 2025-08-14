using System;
using UnityEngine;

class Agent : MonoBehaviour
{
	public enum States
	{
		Patrol,
		Chase,
		Attack,
		BackToPatrol
	}

	public enum Flags
	{
		OnSeeTarget,
		OnTargetReach,
		OnTargetLost,
		OnTargetDied,
		OnBackToPatrolReach
	}

	public FSM<States, Flags> fsm;

	public void Start()
	{
		fsm = new FSM<States, Flags>(States.Patrol);

		fsm.AddState<PatrolState>(States.Patrol);
		fsm.AddState<ChaseState>(States.Chase);
		fsm.AddState<AttackState>(States.Attack);
		fsm.AddState<BackToPatrolState>(States.BackToPatrol);

		fsm.SetTransition(States.Patrol, Flags.OnSeeTarget, States.Chase);
		fsm.SetTransition(States.Chase, Flags.OnTargetReach, States.Attack);
		fsm.SetTransition(States.Chase, Flags.OnTargetLost, States.BackToPatrol);
		fsm.SetTransition(States.Attack, Flags.OnTargetDied, States.BackToPatrol);
		fsm.SetTransition(States.Attack, Flags.OnTargetLost, States.Chase);
		fsm.SetTransition(States.BackToPatrol, Flags.OnBackToPatrolReach, States.Patrol);
		fsm.SetTransition(States.BackToPatrol, Flags.OnSeeTarget, States.Chase);
	}

	private void Update()
	{
		fsm.Tick();
	}

	[ContextMenu("OnSeeTarget")]
	public void TriggerOnSeeTarget()
	{
		fsm.Transition(Flags.OnSeeTarget);
	}

	[ContextMenu("OnTargetReach")]
	public void TriggerOnTargetReach()
	{
		fsm.Transition(Flags.OnTargetReach);
	}

	[ContextMenu("OnTargetLost")]
	public void TriggerOnTargetLost()
	{
		fsm.Transition(Flags.OnTargetLost);
	}

	[ContextMenu("OnTargetDied")]
	public void TriggerOnTargetDied()
	{
		fsm.Transition(Flags.OnTargetDied);
	}

	[ContextMenu("OnBackToPatrolReach")]
	public void TriggerOnBackToPatrolReach()
	{
		fsm.Transition(Flags.OnBackToPatrolReach);
	}
	
}
