namespace ZooArchitect.Architecture.GameLogic
{
    public struct DayStep
    {
        public string name;
        public float duration;

        public DayStep(string name, float duration)
        {
            this.name = name;
            this.duration = duration;
        }
    }
}
