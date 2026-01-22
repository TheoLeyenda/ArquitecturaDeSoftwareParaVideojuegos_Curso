using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public sealed class Worker : Human 
    {
        private Worker(uint ID, Coordinate coordinate) : base(ID, coordinate)
        {

        }
    }
}
