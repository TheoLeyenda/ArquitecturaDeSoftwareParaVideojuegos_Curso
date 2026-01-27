using System;
using ZooArchitect.Architecture.Entities;

namespace ZooArchitect.View.Entities
{
    internal sealed class InfrastructureView : StrcutureView
    {
        public override Type ArchitectureEntityType => typeof(Infrastructure);
    }

}
