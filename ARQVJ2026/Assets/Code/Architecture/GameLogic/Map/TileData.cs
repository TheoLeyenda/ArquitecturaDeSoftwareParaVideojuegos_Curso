using ImageCampus.ToolBox.Blueprints;

namespace ZooArchitect.Architecture.GameLogic
{
    public struct TileData
    {
        [BlueprintParameter("Is Structure")] public bool isStructure;
        [BlueprintParameter("Is Walkable")] public bool isWalkable;
        [BlueprintParameter("Is Animal Habitat")] public bool isAnimalHabitat;
        [BlueprintParameter("Is Animal Habitat Wall")] public bool isAnimalHabitatWall;
        [BlueprintParameter("Can Spawn Humans")] public bool canSpawnHumans;
        [BlueprintParameter("Can Dispawn Humans")] public bool canDispawnHumans;
        [BlueprintParameter("Is Default")] public bool isDefault;
        [BlueprintParameter("Is Unique")] public bool isUnique;
    }
}
