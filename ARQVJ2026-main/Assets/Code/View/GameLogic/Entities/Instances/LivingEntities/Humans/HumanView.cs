using System;
using ZooArchitect.Architecture.Entities;

namespace ZooArchitect.View.Entities
{
    internal abstract class HumanView : LivingEntityView 
    {
        public override Type ArchitectureEntityType => typeof(Human);
    }

}
