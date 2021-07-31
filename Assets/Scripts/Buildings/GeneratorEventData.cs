public class GeneratorEventData
{
    public object GeneratedObject;
    public IPathfindingNode GenerationOrigin;

    public GeneratorEventData() { }

    public GeneratorEventData(object go)
    {
        GeneratedObject = go;
    }

    public GeneratorEventData(object go, IPathfindingNode origin)
    {
        GeneratedObject = go;
        GenerationOrigin = origin;
    }
}