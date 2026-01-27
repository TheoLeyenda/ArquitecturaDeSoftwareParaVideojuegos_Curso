using System;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Entities
{
    [ViewOf(typeof(LivingEntity))]
    internal abstract class LivingEntityView : EntityView 
    {
        public override Type ArchitectureEntityType => typeof(LivingEntity);
    }

}
