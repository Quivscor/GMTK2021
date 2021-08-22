using System.Collections.Generic;

public delegate void RechargingEvent(BuildingEventData e);

public interface IEnergetics : IPathfindingNode
{
    //Max resistance is always base + bonus resistance stat
    public float CurrentResistance { get; }

    public bool IsRecharging();
    public event RechargingEvent OnEnterRechargingState;

    public float ConnectionDamageModifier { get; }
    public float ConnectionSpeedModifier { get; }
}