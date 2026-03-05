using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Data;

namespace ZooArchitect.Architecture.GameLogic
{
    public sealed class Time : IService, ITickable, IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private BlueprintRegistry BlueprintRegistry => ServiceProvider.Instance.GetService<BlueprintRegistry>();
        private BlueprintBinder BlueprintBinder => ServiceProvider.Instance.GetService<BlueprintBinder>();
        public bool IsPersistance => false;

        private float lastDeltaTime;
        private float timeMultiplier;
        public float LogicDeltaTime => lastDeltaTime * timeMultiplier;
        public float TimeMultiplier => timeMultiplier; 


        private List<TimeScaleData> timeScaleDatas;
        private int index;
        public Time()
        {
            timeScaleDatas = new List<TimeScaleData>();

            foreach (string timeBlueprint in BlueprintRegistry.BlueprintsOf(TableNames.TIME_SCALES_TABLE_NAME))
            {
                object timeScaleDataObj = new TimeScaleData();
                BlueprintBinder.Apply(ref timeScaleDataObj, TableNames.TIME_SCALES_TABLE_NAME, timeBlueprint);
                TimeScaleData timeScaleData = (TimeScaleData)timeScaleDataObj;
                if (timeScaleData.isDefault)
                {
                    timeMultiplier = timeScaleData.scale;
                    index = timeScaleDatas.Count;
                }
                timeScaleDatas.Add(timeScaleData);
            }

            EventBus.Subscribe<ChangeTimeScaleRequestAcceptedEvent>(ChangeTimeScale);
        }

        private void ChangeTimeScale(in ChangeTimeScaleRequestAcceptedEvent callback)
        {
            index = (index + 1) % timeScaleDatas.Count;
            timeMultiplier = timeScaleDatas[index].scale;
        }

        public void Tick(float deltaTime)
        {
            lastDeltaTime = deltaTime;
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<ChangeTimeScaleRequestAcceptedEvent>(ChangeTimeScale);
        }

        private struct TimeScaleData
        {
            [BlueprintParameter("Scale")] public float scale;
            [BlueprintParameter("Is default")] public bool isDefault;
        }
    }
}
