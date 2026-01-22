using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public abstract class Human : LivingEntity 
    {
        protected Human(uint ID, Coordinate coordinate) : base(ID, coordinate)
        {

        }
    }
}
