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
            depositRate = depositRate,
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

