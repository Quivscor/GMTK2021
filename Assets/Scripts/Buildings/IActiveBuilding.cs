using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void EnergyEvent();

public interface IActiveBuilding : IEnergetics
{
    public float Energy { get; }
    public float MaxEnergy { get; }
    public void AddEnergy(float energy);
    public bool TryConsumeEnergy(float energy);

    public event EnergyEvent OnReceiveEnergy;
    public event EnergyEvent OnUseEnergy;
}
