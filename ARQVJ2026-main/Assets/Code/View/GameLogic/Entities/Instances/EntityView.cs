using ImageCampus.ToolBox.Services;
using System;
using UnityEngine;
using ZooArchitect.Architecture.Entities;

namespace ZooArchitect.View.Entities
{
    internal abstract class EntityView : MonoBehaviour
    {
        protected EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();
        public abstract Type ArchitectureEntityType { get; }


        protected uint archiectureEntitiyID;
        public uint ArchitectureEnitityID => archiectureEntitiyID;
        protected Entity ArchitectureEntity => EntityRegistry.GetAs<Entity>(archiectureEntitiyID);
    }
}
