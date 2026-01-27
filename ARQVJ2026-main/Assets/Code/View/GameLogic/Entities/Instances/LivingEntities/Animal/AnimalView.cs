using System;
using ZooArchitect.Architecture.Entities;

namespace ZooArchitect.View.Entities
{
    internal sealed class AnimalView : LivingEntityView 
    {
        public override Type ArchitectureEntityType => typeof(Animal);
    }

}
