using System;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Entities
{
    [ViewOf(typeof(Worker))]

    internal sealed class WorkerView : HumanView 
    {
        public override Type ArchitectureEntityType => typeof(Worker);
    }

}
