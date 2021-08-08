[System.Serializable]
public class BuildingStats
{
    public int cost; //building cost

    public float power;
    public float frequency; //speed, cooldowns, etc
    public float electricUsage; //network electricity requirement

    public float rechargeRate; //in case of damaging, recharge time after shutdown
    public float resistance; //effective hp for damaging

    public BuildingStats() { }

    public BuildingStats(int cost, float power, float frequency, float electricUsage, float rechargeRate, float resistance)
    {
        this.cost = cost;
        this.power = power;
        this.frequency = frequency;
        this.electricUsage = electricUsage;
        this.rechargeRate = rechargeRate;
        this.resistance = resistance;
    }

    public void Reset()
    {
        this.cost = 0;
        this.power = 0;
        this.frequency = 0;
        this.electricUsage = 0;
        this.rechargeRate = 0;
        this.resistance = 0;
    }

    public static BuildingStats operator +(BuildingStats a, BuildingStats b)
    {
        return new BuildingStats(a.cost + b.cost, a.power + b.power, a.frequency + b.frequency, a.electricUsage + b.electricUsage,
            a.rechargeRate + b.rechargeRate, a.resistance + b.resistance);
    }

    public static BuildingStats operator -(BuildingStats a, BuildingStats b)
    {
        return new BuildingStats(a.cost - b.cost, a.power - b.power, a.frequency - b.frequency, a.electricUsage - b.electricUsage,
            a.rechargeRate - b.rechargeRate, a.resistance - b.resistance);
    }

    public static BuildingStats operator *(BuildingStats a, BuildingStats b) =>
        new BuildingStats(a.cost * b.cost, a.power * b.power, a.frequency * b.frequency, a.electricUsage * b.electricUsage,
            a.rechargeRate * b.rechargeRate, a.resistance * b.resistance);

    public static BuildingStats operator /(BuildingStats a, float b) =>
        new BuildingStats(a.cost / (int)b, a.power / b, a.frequency / b, a.electricUsage / b, a.rechargeRate / b, a.resistance / b);
}