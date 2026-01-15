using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Scheduling;
using ImageCampus.ToolBox.Services;
using System.Collections.Generic;
using ZooArchitect.Architecture.GameLogic.Events;

namespace ZooArchitect.Architecture.GameLogic
{
    public sealed class DayNightCycle : IService
    {
        public bool IsPersistance => false;
        private TaskScheduler TaskScheduler => ServiceProvider.Instance.GetService<TaskScheduler>();
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        private const int DAY_DURATION = 24;
        private const int DAY_STEPS = 6;
        private const int DAY_STEP_DURATION = DAY_DURATION / DAY_STEPS;

        private const int HOUR_DURATION = 60;

        private readonly List<DayStep> daySteps;
        private int currentStep;

        public DayStep CurrentDayStep => daySteps[currentStep];

        public DayNightCycle()
        {
            currentStep = 0;
            daySteps = new List<DayStep>();
            daySteps.Add(new DayStep("Mañana", DAY_STEP_DURATION));
            daySteps.Add(new DayStep("Mediodía", DAY_STEP_DURATION));
            daySteps.Add(new DayStep("Tarde", DAY_STEP_DURATION));
            daySteps.Add(new DayStep("Atardecer", DAY_STEP_DURATION));
            daySteps.Add(new DayStep("Anochecer", DAY_STEP_DURATION));
            daySteps.Add(new DayStep("Madrugada", DAY_STEP_DURATION));

            TaskScheduler.Schedule(ChangeStep, DAY_STEP_DURATION * HOUR_DURATION);
        }

        private void ChangeStep()
        {
            currentStep = (currentStep + 1) % daySteps.Count;
            TaskScheduler.Schedule(ChangeStep, DAY_STEP_DURATION * HOUR_DURATION);
            EventBus.Raise<DayStepChangeEvent>();
        }
    }
}
