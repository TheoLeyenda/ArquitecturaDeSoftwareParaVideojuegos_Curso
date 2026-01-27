using ImageCampus.ToolBox.Scheduling;
using ImageCampus.ToolBox.Services;
using ImageCampus.ToolBox.Dataflow;

namespace ZooArchitect.Architecture.GameLogic
{
    public sealed class Time : IService, ITickable
    {
        public bool IsPersistance => false;

        private float lastDeltaTime;
        private float timeMultiplier;
        public float LogicDeltaTime => lastDeltaTime * timeMultiplier;

        public Time()
        {
            timeMultiplier = 1.0f;
        }

        public void Tick(float deltaTime)
        {
            lastDeltaTime = deltaTime;
        }
    }
}
