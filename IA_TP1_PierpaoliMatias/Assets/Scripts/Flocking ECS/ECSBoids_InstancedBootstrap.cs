using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ECSBoids_InstancedBootstrap : MonoBehaviour
{
    [Header("Spawn")]
    public int boidCount = 2000;
    public Vector2 spawnMin = new Vector2(-25, -25);
    public Vector2 spawnMax = new Vector2(25, 25);

    [Header("Boid Settings (globales)")]
    public float speed = 5f;
    public float turnSpeed = 6f;
    public float detectionRadius = 2.5f;
    public float alignmentWeight = 1.0f;
    public float cohesionWeight = 0.6f;
    public float separationWeight = 1.2f;
    public float targetWeight = 0.4f;

    [Header("Target (opcional)")]
    public Transform target;

    [Header("Render instancing")]
    public GameObject prefab;
    private Mesh prefabMesh;
    private Material prefabMaterial;
    private Vector3 prefabScale = Vector3.one;
    private const int MAX_OBJS_PER_DRAWCALL = 1000;

    private List<uint> boidIDs;
    private uint targetID;
    private bool hasTargetEntity = false;

    void Start()
    {
        ECSManager.Init();

        boidIDs = new List<uint>(boidCount);

        if (target != null)
        {
            targetID = ECSManager.CreateEntity();
            ECSManager.AddComponent<PositionComponent>(targetID, new PositionComponent(target.position.x, target.position.y, 0f));
            ECSManager.AddComponent<BoidTargetTag>(targetID, new BoidTargetTag());
            hasTargetEntity = true;
        }

        // Crear boids ECS
        for (int i = 0; i < boidCount; i++)
        {
            var id = ECSManager.CreateEntity();

            Vector2 p = new Vector2(
                Random.Range(spawnMin.x, spawnMax.x),
                Random.Range(spawnMin.y, spawnMax.y)
            );

            // Direccion inicial aleatoria
            float a = Random.value * Mathf.PI * 2f;
            Vector2 d = new Vector2(Mathf.Cos(a), Mathf.Sin(a));

            ECSManager.AddComponent<PositionComponent>(id, new PositionComponent(p.x, p.y, 0f));
            ECSManager.AddComponent<ForwardComponent>(id, new ForwardComponent(d.x, d.y, 0f));
            ECSManager.AddComponent<BoidComponent>(id, new BoidComponent(
                speed, turnSpeed, detectionRadius,
                alignmentWeight, cohesionWeight, separationWeight, targetWeight
            ));

            boidIDs.Add(id);
        }

        // Cache de mesh/material/scale del prefab
        prefabMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
        prefabMaterial = prefab.GetComponent<MeshRenderer>().sharedMaterial;
        prefabScale = prefab.transform.localScale;
    }

    void Update()
    {
        // Sincroniza el target (si existe)
        if (hasTargetEntity && target != null)
        {
            var p = ECSManager.GetComponent<PositionComponent>(targetID);
            p.X = target.position.x;
            p.Y = target.position.y;
            p.Z = 0f;
        }

        ECSManager.Tick(Time.deltaTime);
    }

    void LateUpdate()
    {
        // Render por instancing con orientación (up -> forward del boid)
        var positions = ECSManager.GetComponents<PositionComponent>();
        var forwards = ECSManager.GetComponents<ForwardComponent>();

        // Preparar batches
        List<Matrix4x4[]> batches = new List<Matrix4x4[]>();
        int remaining = boidIDs.Count;
        for (int i = 0; i < boidIDs.Count; i += MAX_OBJS_PER_DRAWCALL)
        {
            int count = remaining > MAX_OBJS_PER_DRAWCALL ? MAX_OBJS_PER_DRAWCALL : remaining;
            batches.Add(new Matrix4x4[count]);
            remaining -= MAX_OBJS_PER_DRAWCALL;
        }

        Parallel.For(0, boidIDs.Count, i =>
        {
            uint id = boidIDs[i];
            var pos = positions[id];
            var fwd = forwards[id];

            Vector3 p = new Vector3(pos.X, pos.Y, pos.Z);
            Vector3 upDir = new Vector3(fwd.X, fwd.Y, fwd.Z);
            if (upDir.sqrMagnitude < 1e-6f) upDir = Vector3.up;

            // Hacemos que el “up” del mesh mire a la dirección del boid (plano XY)
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, upDir);

            batches[i / MAX_OBJS_PER_DRAWCALL][i % MAX_OBJS_PER_DRAWCALL].SetTRS(p, rot, prefabScale);
        });

        // Dibujar
        for (int i = 0; i < batches.Count; i++)
        {
            Graphics.DrawMeshInstanced(prefabMesh, 0, prefabMaterial, batches[i]);
        }
    }
}
