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
		private GameObject titlePrefab;
		private Canvas canvas;

		private List<Button> spawnedButtons;
		private Vector2 padding = new Vector2(10, 10);

		private const int BUTTON_HEIGHT = 30;

		private Text titleText;
		private RectTransform titleRect;

		public override void Init()
		{
			spawnedButtons = new List<Button>();
		}

		public override void Init(params object[] parameters)
		{
			base.Init(parameters);

			GameObject containerPrefab = parameters[0] as GameObject;
			buttonPrefab = parameters[1] as GameObject;
			titlePrefab = parameters[2] as GameObject;
			canvas = parameters[3] as Canvas;

			container = Instantiate(containerPrefab, canvas.transform).transform as RectTransform;

			InstantiateTitle();
		}

		private void InstantiateTitle()
		{
			GameObject titleInstance = Instantiate(titlePrefab, container);
			titleText = titleInstance.GetComponent<Text>();
			titleRect = titleInstance.GetComponent<RectTransform>();

			titleRect.anchorMin = new Vector2(0.0f, 1.0f);
			titleRect.anchorMax = new Vector2(0.0f, 1.0f);
			titleRect.pivot = new Vector2(0.0f, 1.0f);

			titleText.text = string.Empty;
			titleInstance.SetActive(false);
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
			titleText.text = string.Empty;
			titleRect.gameObject.SetActive(false);
		}

		public void Show(Dictionary<string, Action> entries)
		{
			Show(null, entries);
		}

		public void Show(string title, Dictionary<string, Action> entries)
		{
			ClearAllButtons();

			bool hasTitle = !string.IsNullOrEmpty(title);

			if (hasTitle)
			{
				titleText.text = title;
				titleRect.gameObject.SetActive(true);
			}
			else
			{
				titleRect.gameObject.SetActive(false);
			}

			foreach (KeyValuePair<string, Action> action in entries)
			{
				CreateButton(action.Key, action.Value);
			}

			LayoutElements(hasTitle);
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

		private void LayoutElements(bool hasTitle)
		{
			float titleHeight = hasTitle ? titleRect.sizeDelta.y : 0.0f;
			float height = spawnedButtons.Count * BUTTON_HEIGHT + padding.y * 2 + titleHeight;
			container.sizeDelta = new Vector2(menuWhith, height);
			float currentY = -padding.y;

			if (hasTitle)
			{
				titleRect.sizeDelta = new Vector2(menuWhith - padding.x * 2.0f, titleRect.sizeDelta.y);
				titleRect.anchoredPosition = new Vector2(padding.x, currentY);
				currentY -= titleRect.sizeDelta.y;
			}

			for (int i = 0; i < spawnedButtons.Count; i++)
			{
				RectTransform rect = spawnedButtons[i].GetComponent<RectTransform>();

				rect.anchorMin = new Vector2(0.0f, 1.0f);
				rect.anchorMax = new Vector2(0.0f, 1.0f);
				rect.pivot = new Vector2(0.0f, 1.0f);

				rect.sizeDelta = new Vector2(menuWhith - padding.x * 2.0f, BUTTON_HEIGHT);
				rect.anchoredPosition = new Vector2(padding.x, currentY);

				currentY -= BUTTON_HEIGHT;
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