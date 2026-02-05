using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Math;
using ZooArchitect.View.Mapping;
using ZooArchitect.View.Scene;

namespace ZooArchitect.View.Entities
{
    [ViewOf(typeof(Entity))]
    internal abstract class EntityView : ViewComponent
    {
        protected EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();
        protected GameScene GameScene => ServiceProvider.Instance.GetService<GameScene>();

        public abstract Type ArchitectureEntityType { get; }

        protected uint archiectureEntitiyID;
        public uint ArchitectureEnitityID => archiectureEntitiyID;
        protected Entity ArchitectureEntity => EntityRegistry.GetAs<Entity>(archiectureEntitiyID);

        public static string SetIdMethodName => nameof(SetId);
        private void SetId(uint ID)
        {
            archiectureEntitiyID = ID;
        }

        public void Move(Coordinate coordinate) 
        {
            transform.position = GameScene.CoordinateToWorld(coordinate);
        }
    }

}
