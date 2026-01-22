using ImageCampus.ToolBox.Blueprints;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public abstract class Entity
    {
        public const uint UNASSIGNED_ENTITY_ID = 0;


        public uint ID;
        public Coordinate coordinate;

        protected Entity(uint ID, Coordinate coordinate) 
        {
            this.ID = ID;
            this.coordinate = coordinate;
        }
    }
}
