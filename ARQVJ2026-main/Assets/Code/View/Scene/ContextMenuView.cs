using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZooArchitect.View.Scene
{
    internal sealed class ContextMenuView : ViewComponent, IService
    {
        public bool IsPersistance => false;

        private float menuWhith = 200.0f;
        private RectTransform container;
        private GameObject buttonPrefab;
        private Canvas canvas;

        private List<Button> spawnedButtons;

        private Vector2 padding = new Vector2(10, 10);
        private const int BUTTON_HEIGHT = 30;
        public override void Init()
        {
            spawnedButtons = new List<Button>();
        }

        public override void Init(params object[] parameters)
        {
            base.Init(parameters);
            GameObject containerPrefab = parameters[0] as GameObject;
            buttonPrefab = parameters[1] as GameObject;
            canvas = parameters[2] as Canvas;

            container = Instantiate(containerPrefab, canvas.transform).transform as RectTransform;
        }

        public override void Tick(float deltaTime)
        {
            if (Input.GetMouseButtonDown(2))
            {
                Hide();
            }
        }

        private void Hide() 
        {
            ClearAllButtons();
        }

        public void Show(Dictionary<string, Action> entries) 
        {
            ClearAllButtons();

            foreach (KeyValuePair<string, Action> action in entries)
            {
                CreateButton(action.Key, action.Value);
            }

            LayoutButtons();
            SetPosition(Input.mousePosition);
        }

        private void SetPosition(Vector2 mousePosition)
        {
            RectTransform canvasRect = canvas.transform as RectTransform;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mousePosition, 
               canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, out localPoint);
            container.anchoredPosition = localPoint;
        }

        private void LayoutButtons()
        {
            float height = spawnedButtons.Count * BUTTON_HEIGHT + padding.y * 2;

            container.sizeDelta = new Vector2(menuWhith, height);

            for (int i = 0; i < spawnedButtons.Count; i++)
            {
                RectTransform rect = spawnedButtons[i].GetComponent<RectTransform>();

                rect.anchorMin = new Vector2(0.0f, 1.0f);
                rect.anchorMax = new Vector2(0.0f, 1.0f);

                rect.pivot = new Vector2(0.0f, 1.0f);

                rect.sizeDelta = new Vector2(menuWhith - padding.x * 2.0f, BUTTON_HEIGHT);

                rect.anchoredPosition = new Vector2(padding.x, -padding.y - i * BUTTON_HEIGHT);
            }


        }

        private void CreateButton(string name, Action action)
        {
            Button button = Instantiate(buttonPrefab, container).GetComponent<Button>();
            spawnedButtons.Add(button);

            Text text = button.GetComponentInChildren<Text>();
            text.text = name;

            button.onClick.AddListener(() => 
            {
                action?.Invoke();
                Hide();
            });
        }

        private void ClearAllButtons() 
        {
            for (int i = 0; i < spawnedButtons.Count; i++)
            {
                spawnedButtons[i].onClick.RemoveAllListeners();
                Destroy(spawnedButtons[i].gameObject);
            }

            spawnedButtons.Clear();
        }

    }
}
