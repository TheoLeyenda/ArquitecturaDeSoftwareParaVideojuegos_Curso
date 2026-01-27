using ImageCampus.ToolBox.Blueprints;

namespace ZooArchitect.Architecture.GameLogic
{
    public struct DayStep
    {
        [BlueprintParameter("Name")] public string name;
        [BlueprintParameter("Duration")] public float duration;

        public DayStep(string name, float duration)
        {
            this.name = name;
            this.duration = duration;
        }
    }
}
