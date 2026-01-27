using System;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Entities
{
    [ViewOf(typeof(Animal))]
    internal sealed class AnimalView : LivingEntityView 
    {
        public override Type ArchitectureEntityType => typeof(Animal);
    }

}
