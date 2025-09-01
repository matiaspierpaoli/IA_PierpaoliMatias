public class ForwardComponent : ECSComponent
{
    public float X;
    public float Y;
    public float Z;

    public ForwardComponent(float x, float y, float z = 0f)
    {
        X = x; Y = y; Z = z;
    }
}
