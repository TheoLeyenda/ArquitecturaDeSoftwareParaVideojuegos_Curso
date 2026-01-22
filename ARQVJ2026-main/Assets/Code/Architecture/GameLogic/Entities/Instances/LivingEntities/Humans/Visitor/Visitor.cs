using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public sealed class Visitor : Human 
    {
        private Visitor(uint ID, Coordinate coordinate) : base(ID, coordinate)
        {

        }
    }
}
