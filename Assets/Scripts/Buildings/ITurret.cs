using System.Collections.Generic;

public delegate void TurretFireEvent();

public interface ITurret
{
    List<Enemy> Targets { get; }

    void Fire();
    void AddTarget(Enemy e);
    void RemoveTarget(Enemy e);

    event TurretFireEvent OnTurretFire;
}