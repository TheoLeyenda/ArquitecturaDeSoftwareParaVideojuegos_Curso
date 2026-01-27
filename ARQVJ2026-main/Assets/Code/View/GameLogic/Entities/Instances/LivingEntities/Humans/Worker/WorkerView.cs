using System;
using ZooArchitect.Architecture.Entities;

namespace ZooArchitect.View.Entities
{
    internal sealed class WorkerView : HumanView 
    {
        public override Type ArchitectureEntityType => typeof(Worker);
    }

}
