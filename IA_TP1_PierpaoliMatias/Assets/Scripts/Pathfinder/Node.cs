public class Node<Coordinate> : INode, INode<Coordinate>
{
    private Coordinate coordinate;
    private bool blocked = false;

    public void SetCoordinate(Coordinate coordinate)
    {
        this.coordinate = coordinate;
    }

    public Coordinate GetCoordinate()
    {
        return coordinate;
    }

    public bool IsBloqued()
    {
        return blocked;
    }

    public void SetBlocked(bool value)
    {
        blocked = value;
    }
}