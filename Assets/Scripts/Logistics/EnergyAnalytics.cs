public class EnergyAnalytics
{
    protected float m_MeasurementTime;
    public float MeasurementTime => m_MeasurementTime;
    public float MeasurementPeriod { get; private set; }
    public float EnergyGained { get; set; }

    public float LastMeasuredAverage { get; private set; }
    public float CurrentMeasuredAverage { get; private set; }

    public EnergyAnalytics(float measurementPeriod)
    {
        MeasurementPeriod = measurementPeriod;
    }

    public void MeasureTime(float deltaTime)
    {
        m_MeasurementTime += deltaTime;
        if (m_MeasurementTime >= MeasurementPeriod)
        {
            LastMeasuredAverage = CurrentMeasuredAverage;
            CurrentMeasuredAverage = GetAverageEnergyGain();
            ClearMeasurements();
        }
    }

    public void MeasureEnergyGain(float energy)
    {
        EnergyGained += energy;
    }

    protected void ClearMeasurements()
    {
        EnergyGained = m_MeasurementTime = 0;
    }

    protected float GetAverageEnergyGain()
    {
        return EnergyGained / MeasurementTime;
    }
}