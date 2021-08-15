using UnityEngine;

[System.Serializable]
public class BuildingShopData
{
    public Sprite UIBuildingSprite;
    [TextArea(4,10)] public string UIBuildingDescription;
    public string UIBuildingName;
    public BuildingShopCategory BuildingShopCategory;
}

public enum BuildingShopCategory
{
    OFFENSIVE = 0,
    MODULES = 1000,
    LOGISTIC = 2000,
    OTHER = 3000,
}