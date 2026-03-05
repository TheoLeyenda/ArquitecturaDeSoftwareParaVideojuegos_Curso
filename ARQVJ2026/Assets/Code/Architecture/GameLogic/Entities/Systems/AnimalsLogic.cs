using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.GameLogic;

namespace ZooArchitect.Architecture.Entities
{
    public sealed class AnimalsLogic : ITickable, IDisposable
    {
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();
        private DayNightCycle DayNightCycle => ServiceProvider.Instance.GetService<DayNightCycle>();
        private Scene Scene => ServiceProvider.Instance.GetService<Scene>();

        public void Tick(float deltaTime)
        {
            CheckForFights();
        }

        private void CheckForFights()
        {
            List<uint> deadEntities = new List<uint>();

            foreach (Animal animal in EntityRegistry.Animals)
            {
                if (deadEntities.Contains(animal.ID))
                    continue;

                foreach (uint entityID in Scene.GetEntitiesIn(animal.coordinate))
                {
                    if (entityID == animal.ID)
                        continue;

                    if (deadEntities.Contains(entityID))
                        continue;

                    if (animal.CanAttackTo(entityID))
                    {
                        deadEntities.Add(animal.ID);
                        deadEntities.Add(entityID);
                    }
                }
            }

            foreach (uint deadEntityId in deadEntities)
            {
                EntityRegistry.Unregister(EntityRegistry[deadEntityId]);
            }
        }

        internal void FeedAnimals()
        {
            foreach (Animal animal in EntityRegistry.Animals)
            {
                animal.Feed();
            }
        }

        internal void CheckSleepHours()
        {
            foreach (Animal animal in EntityRegistry.Animals)
            {
                if (DayNightCycle.IsThisStep(animal.WakeUpHour))
                    animal.WakeUp();
                if (DayNightCycle.IsThisStep(animal.GoToSleepHour))
                    animal.GoToSleep();
            }
        }

        public void Dispose()
        {
        }
    }
}