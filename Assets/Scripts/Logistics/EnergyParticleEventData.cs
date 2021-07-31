public class EnergyParticleEventData
{
    public EnergyParticle Particle;
    public IActiveBuilding Target;

    public EnergyParticleEventData() { }

    public EnergyParticleEventData(EnergyParticle owner)
    {
        Particle = owner;
    }

    public EnergyParticleEventData(EnergyParticle owner, IActiveBuilding target)
    {
        Particle = owner;
        Target = target;
    }
}