using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public abstract class LivingEntity : Entity
    
    {
        protected LivingEntity(uint ID, Coordinate coordinate) : base(ID, coordinate)
        {

        }
    }
}
