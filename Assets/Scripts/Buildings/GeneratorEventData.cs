public class GeneratorEventData
{
    public object GeneratedObject;
    public IPathfindingNode GenerationOrigin;
    public float GeneratedValue;

    public GeneratorEventData() { }

    public GeneratorEventData(object go)
    {
        GeneratedObject = go;
    }

    public GeneratorEventData(float value)
    {
        GeneratedValue = value;
    }

    public GeneratorEventData(object go, IPathfindingNode origin)
    {
        GeneratedObject = go;
        GenerationOrigin = origin;
    }
}