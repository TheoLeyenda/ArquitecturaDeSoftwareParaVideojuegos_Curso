using ImageCampus.ToolBox.Bluprints;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public abstract class Entity
    {
        public const uint UNASSIGNED_ENTITY_ID = 0;

        [BlueprintParameter("Food")] private int food;

        public uint ID;
        public Coordinate coordinate;

        protected Entity(uint ID, Coordinate coordinate) 
        {
            this.ID = ID;
            this.coordinate = coordinate;
        }
    }
}
