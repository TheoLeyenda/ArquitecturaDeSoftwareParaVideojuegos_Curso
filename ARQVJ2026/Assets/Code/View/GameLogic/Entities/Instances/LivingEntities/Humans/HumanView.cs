using System;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Entities
{
    [ViewOf(typeof(Human))]
    internal abstract class HumanView : LivingEntityView 
    {
        public override Type ArchitectureEntityType => typeof(Human);
    }

}
