using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.View.Mapping;
using ZooArchitect.View.Scene;

namespace ZooArchitect.View.Entities
{
    [ViewOf(typeof(Entity))]
    internal abstract class EntityView : ViewComponent
    {
        protected EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();
        public abstract Type ArchitectureEntityType { get; }

        protected uint archiectureEntitiyID;
        public uint ArchitectureEnitityID => archiectureEntitiyID;
        protected Entity ArchitectureEntity => EntityRegistry.GetAs<Entity>(archiectureEntitiyID);

        public static string SetIdMethodName => nameof(SetId);
        private void SetId(uint ID)
        {
            archiectureEntitiyID = ID;
        }
    }

}
