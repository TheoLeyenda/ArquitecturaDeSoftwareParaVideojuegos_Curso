using System;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Entities
{
    [ViewOf(typeof(Jail))]
    internal sealed class JailView : StrcutureView
    {
        public override Type ArchitectureEntityType => typeof(Jail);
    }

}
