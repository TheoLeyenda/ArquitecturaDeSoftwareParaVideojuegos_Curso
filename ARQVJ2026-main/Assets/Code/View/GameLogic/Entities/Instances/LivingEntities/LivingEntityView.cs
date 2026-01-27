using System;
using ZooArchitect.Architecture.Entities;

namespace ZooArchitect.View.Entities
{
    internal abstract class LivingEntityView : EntityView 
    {
        public override Type ArchitectureEntityType => typeof(LivingEntity);
    }

}
