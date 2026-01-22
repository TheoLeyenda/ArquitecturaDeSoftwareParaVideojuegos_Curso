using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public abstract class Structure : Entity 
    {
        protected Structure(uint ID, Coordinate coordinate) : base(ID, coordinate)
        {

        }
    }
}
