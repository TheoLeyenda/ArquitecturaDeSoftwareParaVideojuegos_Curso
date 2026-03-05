using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using UnityEngine;
using UnityEngine.UI;
using ZooArchitect.Architecture.Controllers;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.GameLogic;
using ZooArchitect.View.Mapping;
using Time = ZooArchitect.Architecture.GameLogic.Time;
namespace ZooArchitect.View.Controller
{
    [ViewOf(typeof(TimeScaleController))]
    internal sealed class TimeScaleControllerView : IDisposable
    {
        private Time Time => ServiceProvider.Instance.GetService<Time>();
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        private Button controllerButton;
        private Text buttonText;
        public TimeScaleControllerView(Button controllerButton)
        {
            this.controllerButton = controllerButton;
            (this.controllerButton.transform as RectTransform).anchorMin = new Vector2(0, 0);    
            (this.controllerButton.transform as RectTransform).anchorMax = new Vector2(0, 0);  
            (this.controllerButton.transform as RectTransform).pivot = new Vector2(0, 0);

            buttonText = controllerButton.gameObject.GetComponentInChildren<Text>();

            SetButtonText();
            controllerButton.onClick.AddListener(RequestChangeTimeScale);

            EventBus.Subscribe<ChangeTimeScaleRequestAcceptedEvent>(OnChangeTimeScaleAccepted);
        }

        private void OnChangeTimeScaleAccepted(in ChangeTimeScaleRequestAcceptedEvent callback)
        {
            SetButtonText();
        }

        private void SetButtonText() 
        {
            buttonText.text = string.Concat("<color=black>X",Time.TimeMultiplier.ToString(),"</color>");
        }

        private void RequestChangeTimeScale()
        {
            EventBus.Raise<ChangeTimeScaleRequestEvent>();
        }

        public void Dispose()
        {
            controllerButton.onClick.RemoveAllListeners();
            EventBus.Unsubscribe<ChangeTimeScaleRequestAcceptedEvent>(OnChangeTimeScaleAccepted);
        }

    }
}
