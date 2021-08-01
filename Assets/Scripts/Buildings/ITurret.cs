using System.Collections.Generic;

public delegate void TurretFireEvent();

public interface ITurret : IActiveBuilding
{
    List<Enemy> Targets { get; }

    bool Fire();
    void AddTarget(Enemy e);
    void RemoveTarget(Enemy e);

    event TurretFireEvent OnTurretFire;
}