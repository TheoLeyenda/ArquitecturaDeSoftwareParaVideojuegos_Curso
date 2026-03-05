using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.View.Feedback;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Entities
{
	[ViewOf(typeof(Animal))]
	internal sealed class AnimalView : LivingEntityView
	{
		private FeedbackFactory FeedbackFactory => ServiceProvider.Instance.GetService<FeedbackFactory>();

		public override Type ArchitectureEntityType => typeof(Animal);

		[BlueprintParameter("Feed sucsess feedback")] private string feedSucsessFeedbackKey;
		[BlueprintParameter("Feed fail feedback")] private string feedFailFeedbackKey;

		internal void OnFeedSucsess()
		{
			FeedbackFactory.Spawn(feedSucsessFeedbackKey, transform.position);
		}

		internal void OnFeedFail()
		{
			FeedbackFactory.Spawn(feedFailFeedbackKey, transform.position);
		}
	}

}
