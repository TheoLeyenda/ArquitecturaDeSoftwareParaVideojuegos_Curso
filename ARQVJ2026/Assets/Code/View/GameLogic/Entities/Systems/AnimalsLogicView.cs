using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Entities
{
    [ViewOf(typeof(AnimalsLogic))]
    internal sealed class AnimalsLogicView : IInitable, ITickable, IDisposable
    {
        private EntityRegistryView EntityRegistryView => ServiceProvider.Instance.GetService<EntityRegistryView>();

        public void Init()
        {
        }

        public void LateInit()
        {
        }

        public void Tick(float deltaTime)
        {
        }
        public void Dispose()
        {
        }

        internal void OnFeedAnimalSucsess(uint animalID)
        {
            EntityRegistryView.GetAs<AnimalView>(animalID).OnFeedSucsess();
        }

        internal void OnFeedAnimalFail(uint animalID)
        {
            EntityRegistryView.GetAs<AnimalView>(animalID).OnFeedFail();
        }
    }
}
