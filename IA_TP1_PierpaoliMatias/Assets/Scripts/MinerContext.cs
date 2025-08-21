using UnityEngine;

public class MinerContext
{
    public Transform agent;
    public Transform mine;
    public Transform home;

    public float speed;
    public float arrivalRadius;

    public float gatherRate;
    public float depositRate;

    public int capacity;
    public int inventory;
    public int mineRemaining;

    public float gatherAccumulator = 0f;
    public float depositAccumulator = 0f;
}