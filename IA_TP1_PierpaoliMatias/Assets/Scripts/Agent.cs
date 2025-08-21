//using System;
//using UnityEngine;

//class Agent : MonoBehaviour
//{
//    public enum States
//    {
//        Patrol,
//        Chase,
//        Explode
//    }

//    public enum Flags
//    {
//        OnTargetReach,
//        OnTargetNear,
//        OnTargetLost
//    }

//    public FSM<States, Flags> fsm;

//    public Transform target;
//    public float speed;
//    public float explodeDistance;
//    public float lostDistance;

//    public Transform wayPoint1;
//    public Transform wayPoint2;
//    public float chaseDistance;

//    public void Start()
//    {
//        fsm = new FSM<States, Flags>(States.Patrol);

//        fsm.AddState<PatrolState>(States.Patrol,
//            onTickParameters: () => new object[] { wayPoint1, wayPoint2, transform, target, speed, chaseDistance, Time.deltaTime }
//            );

//        fsm.AddState<ChaseState>(States.Chase,
//            onTickParameters: () => new object[] { transform, target, speed, explodeDistance, lostDistance, Time.deltaTime }
//            );

//        fsm.AddState<ExplodeState>(States.Explode);

//        fsm.SetTransition(States.Patrol, Flags.OnTargetNear, States.Chase, () => { Debug.Log("Te vi"); });
//        fsm.SetTransition(States.Chase, Flags.OnTargetReach, States.Explode);
//        fsm.SetTransition(States.Chase, Flags.OnTargetLost, States.Patrol);
//    }

//    private void Update()
//    {
//        fsm.Tick();
//    }
//}

using UnityEngine;

public class Agent : MonoBehaviour
{
    public enum States { GoToMine, Mine, ReturnToBase, Deposit, Done }
    public enum Flags { ArrivedMine, InventoryFull, MineDepleted, ArrivedBase, NeedToMine, WorkDone }

    public FSM<States, Flags> fsm;

    [Header("Scene Refs")]
    public Transform mine;
    public Transform home;

    [Header("Tuning")]
    public int capacity = 10;
    public int mineAmount = 50;
    public float speed = 5f;
    public float arrivalRadius = 0.2f;
    public float gatherRate = 2f;
    public float depositRate = 8f;

    private MinerContext ctx;

    void Start()
    {
        ctx = new MinerContext
        {
            agent = transform,
            mine = mine,
            home = home,
            speed = speed,
            arrivalRadius = arrivalRadius,
            gatherRate = gatherRate,
            capacity = capacity,
            inventory = 0,
            mineRemaining = mineAmount,
            depositRate = depositRate
        };

        fsm = new FSM<States, Flags>(States.GoToMine);

        fsm.AddState<GoToMineState>(States.GoToMine,
            onTickParameters: () => new object[] { ctx, Time.deltaTime });

        fsm.AddState<MineState>(States.Mine,
            onTickParameters: () => new object[] { ctx, Time.deltaTime });

        fsm.AddState<ReturnToBaseState>(States.ReturnToBase,
            onTickParameters: () => new object[] { ctx, Time.deltaTime });

        fsm.AddState<DepositState>(States.Deposit,
            onTickParameters: () => new object[] { ctx, Time.deltaTime });

        fsm.AddState<DoneState>(States.Done);

        fsm.SetTransition(States.GoToMine, Flags.ArrivedMine, States.Mine);
        fsm.SetTransition(States.Mine, Flags.InventoryFull, States.ReturnToBase);
        fsm.SetTransition(States.Mine, Flags.MineDepleted, States.ReturnToBase);
        fsm.SetTransition(States.ReturnToBase, Flags.ArrivedBase, States.Deposit);
        fsm.SetTransition(States.Deposit, Flags.NeedToMine, States.GoToMine);
        fsm.SetTransition(States.Deposit, Flags.WorkDone, States.Done);
    }

    void Update() => fsm.Tick();
}

