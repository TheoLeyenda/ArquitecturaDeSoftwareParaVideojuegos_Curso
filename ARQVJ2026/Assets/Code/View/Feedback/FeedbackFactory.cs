using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Services;
using UnityEngine;
using ZooArchitect.View.Data;
using ZooArchitect.View.Resources;
using ZooArchitect.View.Scene;

namespace ZooArchitect.View.Feedback
{
	internal sealed class FeedbackFactory : IService
	{
		public bool IsPersistance => false;

		private PrefabsRegistryView PrefabsRegistryView => ServiceProvider.Instance.GetService<PrefabsRegistryView>();
		private BlueprintBinder BlueprintBinder => ServiceProvider.Instance.GetService<BlueprintBinder>();

		private GameScene GameScene => ServiceProvider.Instance.GetService<GameScene>();

		private const string POSITIVE_FEEDBACK_KEY = "Positive";
		private const string NEGATIVE_FEEDBACK_KEY = "Negative";

		public FeedbackFactory() { }

		public void Spawn(string feedbackId, Vector3 position)
		{
			GameObject prefab = PrefabsRegistryView.Get(TableNamesView.FEEDBACK_VIEW_TABLE_NAME, feedbackId);
			GameObject instance = UnityEngine.Object.Instantiate(prefab, 
				position + new Vector3(GameScene.GetCellSize.x / 2, GameScene.GetCellSize.y / 2),
				Quaternion.identity);
			object feedbackInstance = instance.GetComponent<FloatAndFadeFeedback>();
			BlueprintBinder.Apply(ref feedbackInstance, TableNamesView.FEEDBACK_VIEW_TABLE_NAME,
				PrefabsRegistryView.ArchiectureToViewId(TableNamesView.FEEDBACK_VIEW_TABLE_NAME, feedbackId));
			(feedbackInstance as FloatAndFadeFeedback).Init();
			(feedbackInstance as FloatAndFadeFeedback).LateInit();
		}

		public void SpawnPositiveFeedback(Vector3 position)
		{
			Spawn(POSITIVE_FEEDBACK_KEY, position);
		}

		public void SpawnNegativeFeedback(Vector3 position) 
		{
			Spawn(NEGATIVE_FEEDBACK_KEY, position);
		}
	}
}
