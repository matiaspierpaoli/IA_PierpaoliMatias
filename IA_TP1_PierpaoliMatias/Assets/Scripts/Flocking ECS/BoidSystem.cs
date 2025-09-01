using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine; // Solo para Mathf

public sealed class BoidSystem : ECSSystem
{
    private ParallelOptions parallelOptions;

    private IDictionary<uint, PositionComponent> positionComponents;
    private IDictionary<uint, ForwardComponent> forwardComponents;
    private IDictionary<uint, BoidComponent> boidComponents;

    private IEnumerable<uint> boidEntities;
    private IEnumerable<uint> targetEntities;

    // Hash espacial 2D
    private struct Cell2D : IEquatable<Cell2D>
    {
        public int x, y;
        public Cell2D(int x, int y) { this.x = x; this.y = y; }

        public bool Equals(Cell2D other) => x == other.x && y == other.y;
        public override bool Equals(object obj) => obj is Cell2D o && Equals(o);
        public override int GetHashCode() => (x * 73856093) ^ (y * 19349663);
    }

    // Construido en PreExecute, usado en Execute (solo lectura allí)
    private Dictionary<Cell2D, List<uint>> grid;
    private float cellSize; // tomamos un tamano de celda >= detectionRadius (asumimos radios homogeneos)

    public override void Initialize()
    {
        parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 32 };
        grid = new Dictionary<Cell2D, List<uint>>(1024);
        cellSize = 1.0f; // se ajusta en PreExecute con el primer boid
    }

    protected override void PreExecute(float deltaTime)
    {
        positionComponents ??= ECSManager.GetComponents<PositionComponent>();
        forwardComponents ??= ECSManager.GetComponents<ForwardComponent>();
        boidComponents ??= ECSManager.GetComponents<BoidComponent>();

        boidEntities ??= ECSManager.GetEntitiesWhitComponentTypes(
            typeof(PositionComponent), typeof(ForwardComponent), typeof(BoidComponent));

        targetEntities ??= ECSManager.GetEntitiesWhitComponentTypes(
            typeof(PositionComponent), typeof(BoidTargetTag));

        // Ajustamos cellSize con el primer boid (asumimos radio homogeneo para performance)
        using (var enumerator = boidEntities.GetEnumerator())
        {
            if (enumerator.MoveNext())
            {
                var firstID = enumerator.Current;
                cellSize = Math.Max(0.001f, boidComponents[firstID].detectionRadius);
            }
        }

        // Construimos el hash espacial (secuencial para reducir contencion)
        grid.Clear();
        foreach (var kv in boidEntities)
        {
            var pos = positionComponents[kv];
            var cell = WorldToCell(pos.X, pos.Y, cellSize);
            if (!grid.TryGetValue(cell, out var list))
            {
                list = new List<uint>(16);
                grid[cell] = list;
            }
            list.Add(kv);
        }
    }

    protected override void Execute(float deltaTime)
    {
        // Obtenemos el target (si existe)
        Vector2 targetPos = Vector2.zero;
        bool hasTarget = false;
        using (var enumerator = targetEntities.GetEnumerator())
        {
            if (enumerator.MoveNext())
            {
                var tID = enumerator.Current;
                var tPos = positionComponents[tID];
                targetPos = new Vector2(tPos.X, tPos.Y);
                hasTarget = true;
            }
        }

        int range = 1; // buscamos en 3x3 celdas (con cellSize = detectionRadius alcanza)
        float eps = 1e-6f;

        Parallel.ForEach(boidEntities, parallelOptions, id =>
        {
            var boid = boidComponents[id];
            var pos = positionComponents[id];
            var fwd = forwardComponents[id];

            Vector2 p = new Vector2(pos.X, pos.Y);
            Vector2 f = new Vector2(fwd.X, fwd.Y);
            if (f.sqrMagnitude < eps) f = new Vector2(0, 1); // fallback

            Vector2 align = Vector2.zero;
            Vector2 cohes = Vector2.zero;
            Vector2 separ = Vector2.zero;

            int count = 0;

            var c = WorldToCell(p.x, p.y, cellSize);
            for (int dy = -range; dy <= range; dy++)
                for (int dx = -range; dx <= range; dx++)
                {
                    var nCell = new Cell2D(c.x + dx, c.y + dy);
                    if (!grid.TryGetValue(nCell, out var bucket)) continue;

                    for (int i = 0; i < bucket.Count; i++)
                    {
                        uint other = bucket[i];
                        if (other == id) continue;

                        var op = positionComponents[other];
                        var of = forwardComponents[other];

                        Vector2 op2 = new Vector2(op.X, op.Y);
                        float distSqr = (op2 - p).sqrMagnitude;

                        if (distSqr < boid.detectionRadius * boid.detectionRadius)
                        {
                            count++;
                            align += new Vector2(of.X, of.Y); // forward del vecino
                            cohes += op2;
                            Vector2 diff = (p - op2);
                            float inv = 1.0f / (Mathf.Sqrt(distSqr) + eps);
                            separ += diff * inv; // empuje inverso a la distancia
                        }
                    }
                }

            Vector2 dirAlignment = Vector2.zero;
            Vector2 dirCohesion = Vector2.zero;
            Vector2 dirSeparation = Vector2.zero;

            if (count > 0)
            {
                dirAlignment = (align / count).normalized;
                dirCohesion = ((cohes / count) - p).normalized;
                dirSeparation = (separ / count).normalized;
            }

            Vector2 dirTarget = Vector2.zero;
            if (hasTarget)
            {
                dirTarget = (targetPos - p).normalized;
            }

            // ACS + dirección al objetivo
            Vector2 desired =
                (dirAlignment * boid.alignmentWeight) +
                (dirCohesion * boid.cohesionWeight) +
                (dirSeparation * boid.separationWeight) +
                (dirTarget * boid.targetWeight);

            if (desired.sqrMagnitude < eps) desired = f; // si no hay estimulo, mantener rumbo
            else desired.Normalize();

            // Lerp hacia la direccion deseada (tipo "slerp" plano)
            float t = 1f - Mathf.Exp(-boid.turnSpeed * deltaTime); // suave, independiente de FPS
            Vector2 newF = Vector2.Lerp(f, desired, t).normalized;

            // Integracion
            p += newF * (boid.speed * deltaTime);

            // Escribimos resultados
            positionComponents[id].X = p.x;
            positionComponents[id].Y = p.y;
            // Z permanece 0

            forwardComponents[id].X = newF.x;
            forwardComponents[id].Y = newF.y;
            forwardComponents[id].Z = 0f;
        });
    }

    protected override void PostExecute(float deltaTime) { }

    private static Cell2D WorldToCell(float x, float y, float h)
    {
        int cx = Mathf.FloorToInt(x / h);
        int cy = Mathf.FloorToInt(y / h);
        return new Cell2D(cx, cy);
    }
}
