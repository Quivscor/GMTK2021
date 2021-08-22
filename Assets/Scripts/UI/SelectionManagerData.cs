using System;

public class SelectionManagerData
{
    public Action OnBuildingSelected;
    public Action OnDeselection;
    public Action OnSelectedBuildingUpdated;

    public Building SelectedBuilding { get; set; }
    private Building m_SelectedBuildingMock;
    private Building m_SelectedBuildingCopy;
    public Building SelectedBuildingMock { get => m_SelectedBuildingMock; set => m_SelectedBuildingMock = value; }
    public Building SelectedBuildingCopy { get => m_SelectedBuildingCopy; set => m_SelectedBuildingCopy = value; }
    public GridField SelectedBuildingMockField;
}