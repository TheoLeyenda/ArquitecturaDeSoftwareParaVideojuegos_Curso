using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Entities.Events;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Entities
{
    [ViewOf(typeof(EntitiesLogic))]
    internal sealed class EntitiesLogicView : IInitable ,ITickable, IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        private EntityRegistryView EntityRegistryView => ServiceProvider.Instance.GetService<EntityRegistryView>();
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();

        private AnimalsLogicView animalsLogicView;
        public EntitiesLogicView()
        {
            animalsLogicView = new AnimalsLogicView();
            EventBus.Subscribe<EntityMovedEvent>(OnEntityMoved);
            EventBus.Subscribe<OnAnimalFeedSucsess>(AnimalFeedSucsess);
            EventBus.Subscribe<OnAnimalFeedFail>(AnimalFeedFail);
        }

        private void AnimalFeedSucsess(in OnAnimalFeedSucsess onAnimalFeedSucsess)
        {
            animalsLogicView.OnFeedAnimalSucsess(onAnimalFeedSucsess.animalID);
        }

        private void AnimalFeedFail(in OnAnimalFeedFail onAnimalFeedFail)
        {
            animalsLogicView.OnFeedAnimalFail(onAnimalFeedFail.animalID);
        }

        private void OnEntityMoved(in EntityMovedEvent entityMovedEvent)
        {
            EntityRegistryView.GetAs<EntityView>(entityMovedEvent.movedEntityId).
                Move(EntityRegistry.GetAs<Entity>(entityMovedEvent.movedEntityId).coordinate);
        }
        public void Init()
        {
            animalsLogicView.Init();
        }

        public void LateInit()
        {
            animalsLogicView.LateInit();
        }

        public void Tick(float deltaTime)
        {
            animalsLogicView.Tick(deltaTime);
        }

        public void Dispose()
        {
            animalsLogicView.Dispose();
            EventBus.Unsubscribe<EntityMovedEvent>(OnEntityMoved);
            EventBus.Unsubscribe<OnAnimalFeedSucsess>(AnimalFeedSucsess);
            EventBus.Unsubscribe<OnAnimalFeedFail>(AnimalFeedFail);
        }

    }
}
