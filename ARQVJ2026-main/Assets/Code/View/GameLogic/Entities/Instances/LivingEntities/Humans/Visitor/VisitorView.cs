using System;
using ZooArchitect.Architecture.Entities;

namespace ZooArchitect.View.Entities
{
    internal sealed class VisitorView : HumanView 
    {
        public override Type ArchitectureEntityType => typeof(Visitor);
    }

}
