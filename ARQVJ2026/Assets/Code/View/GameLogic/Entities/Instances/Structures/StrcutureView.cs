using System;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Entities
{
    [ViewOf(typeof(Structure))]
    internal abstract class StrcutureView : EntityView 
    {
        public override Type ArchitectureEntityType => typeof(Structure);
    }

}
