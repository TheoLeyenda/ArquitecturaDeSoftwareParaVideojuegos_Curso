using System;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Entities
{
    [ViewOf(typeof(Visitor))]

    internal sealed class VisitorView : HumanView 
    {
        public override Type ArchitectureEntityType => typeof(Visitor);
    }

}
