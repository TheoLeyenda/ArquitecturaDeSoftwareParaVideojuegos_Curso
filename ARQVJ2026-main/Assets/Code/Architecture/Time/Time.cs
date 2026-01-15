using ImageCampus.ToolBox.Scheduling;
using ImageCampus.ToolBox.Services;
using ImageCampus.ToolBox.Updateable;

namespace ZooArchitect.Architecture.GameLogic
{
    public sealed class Time : IService, IUpdateable
    {
        public bool IsPersistance => false;

        private float lastDeltaTime;
        private float timeMultiplier;
        public float LogicDeltaTime => lastDeltaTime * timeMultiplier;

        public Time()
        {
            timeMultiplier = 1.0f;
        }

        public void Update(float deltaTime)
        {
            lastDeltaTime = deltaTime;
        }
    }
}
