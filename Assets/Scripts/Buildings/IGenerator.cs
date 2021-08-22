public delegate void GenerateEvent(GeneratorEventData e);
public delegate void GenerationProgressEvent();

public interface IGenerator
{
    public void Generate();

    public float GenerationCooldown { get; }
    public float GenerationCooldownCurrent { get; }
    public string GeneratedObjectName { get; }

    public int ConsumerCount { get; set; }

    event GenerateEvent OnGenerate;
    event GenerationProgressEvent OnGenerationProgress;
}