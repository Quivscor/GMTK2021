public delegate void GenerateEvent(GeneratorEventData e); 

public interface IGenerator
{
    public void Generate(IPathfindingNode origin);

    event GenerateEvent OnGenerate;
}