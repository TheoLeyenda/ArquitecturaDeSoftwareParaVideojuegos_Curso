using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.View.Feedback;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Entities
{
	[ViewOf(typeof(Worker))]
	internal sealed class WorkerView : HumanView
	{
		private FeedbackFactory FeedbackFactory => ServiceProvider.Instance.GetService<FeedbackFactory>();
		public override Type ArchitectureEntityType => typeof(Worker);

		[BlueprintParameter("Working feedback")] private string workingFeedbackKey;

		internal void OnStartWorking()
		{
			FeedbackFactory.Spawn(workingFeedbackKey, transform.position);
		}

		internal void OnEndWorking()
		{
			FeedbackFactory.SpawnPositiveFeedback(transform.position);
		}
	}
}
