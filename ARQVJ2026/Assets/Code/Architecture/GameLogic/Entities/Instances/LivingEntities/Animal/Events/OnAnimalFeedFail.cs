using ImageCampus.ToolBox.Events;

namespace ZooArchitect.Architecture.Entities.Events
{
    public struct OnAnimalFeedFail : IEvent
    {
        public uint animalID;
        public void Assign(params object[] parameters)
        {
            animalID = (uint)parameters[0];
        }

        public void Reset()
        {
            animalID = Entity.UNASSIGNED_ENTITY_ID;
        }
    }
}
