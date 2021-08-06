public delegate void GenerateEvent(GeneratorEventData e); 

public interface IGenerator
{
    public void Generate(IPathfindingNode origin);

    public int ConsumerCount { get; set; }

    event GenerateEvent OnGenerate;
}