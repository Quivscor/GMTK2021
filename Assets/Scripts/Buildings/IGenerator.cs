public delegate void GenerateEvent(GeneratorEventData e); 

public interface IGenerator
{
    public void Generate();

    public int ConsumerCount { get; set; }

    event GenerateEvent OnGenerate;
}