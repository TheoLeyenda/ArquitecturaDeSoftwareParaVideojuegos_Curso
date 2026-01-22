using ImageCampus.ToolBox.Blueprints;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public sealed class Animal : Entity
    {
        [BlueprintParameter("Life")] private int[] life;

        protected Animal(uint ID, Coordinate coordinate) : base(ID, coordinate)
        {

        }
    }
}
