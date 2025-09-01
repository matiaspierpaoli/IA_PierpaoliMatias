public class BoidComponent : ECSComponent
{
    public float speed;            // Magnitud de velocidad
    public float turnSpeed;        // Qué tan rápido interpola hacia la dirección deseada (1/seg)
    public float detectionRadius;  // Radio de vecindad

    public float alignmentWeight;  // Pesos ACSD
    public float cohesionWeight;
    public float separationWeight;
    public float targetWeight;

    public BoidComponent(float speed, float turnSpeed, float detectionRadius,
                         float alignmentWeight, float cohesionWeight, float separationWeight, float targetWeight)
    {
        this.speed = speed;
        this.turnSpeed = turnSpeed;
        this.detectionRadius = detectionRadius;

        this.alignmentWeight = alignmentWeight;
        this.cohesionWeight = cohesionWeight;
        this.separationWeight = separationWeight;
        this.targetWeight = targetWeight;
    }
}
