using System;
using UnityEngine;

class Agent : MonoBehaviour
{
	public enum States
	{
		Patrol,
		Chase,
		Explode
	}

	public enum Flags
	{
		OnTargetReach,
		OnTargetNear,
		OnTargetLost
	}

	public FSM<States, Flags> fsm;

	public Transform target;
	public float speed;
	public float explodeDistance;
	public float lostDistance;

	public Transform myPoint1;
	public Transform myPoint2;
	public float chaseDistance;

	public void Start()
	{
		fsm = new FSM<States, Flags>(States.Patrol);
		fsm.AddState<PatrolState>(States.Patrol,
			onTickParameters: () => new object[] {myPoint1, myPoint2, transform, target, speed, chaseDistance, Time.deltaTime}
			);

		fsm.AddState<ChaseState>(States.Chase,
			onTickParameters: () => new object[] { transform, target, speed, explodeDistance, lostDistance, Time.deltaTime }
			);

		//fsm.AddState<ExlodedState>(States.Explode);

		fsm.SetTransition(States.Patrol, Flags.OnTargetNear, States.Chase, () => { Debug.Log("Te vi"); });
		fsm.SetTransition(States.Chase, Flags.OnTargetNear, States.Chase);
		fsm.SetTransition(States.Chase, Flags.OnTargetLost, States.Patrol);
	}

	private void Update()
	{
		fsm.Tick();
	}
	
}
