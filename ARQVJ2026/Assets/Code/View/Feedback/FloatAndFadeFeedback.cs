using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Scheduling;
using ImageCampus.ToolBox.Services;
using UnityEngine;
using Time = ZooArchitect.Architecture.GameLogic.Time;

namespace ZooArchitect.View.Feedback
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class FloatAndFadeFeedback : MonoBehaviour, IInitable
	{
		private TaskScheduler TaskScheduler => ServiceProvider.Instance.GetService<TaskScheduler>();
		private Time Time => ServiceProvider.Instance.GetService<Time>();

		[BlueprintParameter("Move distance")] private float moveDistance;
		[BlueprintParameter("Duration")] private float duration;

		private SpriteRenderer spriteRenderer;
		private Vector3 startPosition;
		private Vector3 targetPosition;
		private float elapsedTime;
		private Color color;

		private void Awake()
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}

		public void Init()
		{
			startPosition = transform.position;
			targetPosition = startPosition + Vector3.up * moveDistance;

			color = spriteRenderer.color;
			elapsedTime = 0f;

			spriteRenderer.sortingOrder = 2;

			TaskScheduler.Schedule(() => Destroy(gameObject), duration);
		}

		private void Update()
		{
			elapsedTime += Time.LogicDeltaTime;
			float t = elapsedTime / duration;
			transform.position = Vector3.Lerp(startPosition, targetPosition, t);
			color.a = 1f - t;
			spriteRenderer.color = color;
		}

		public void LateInit()
		{
		}
	}
}