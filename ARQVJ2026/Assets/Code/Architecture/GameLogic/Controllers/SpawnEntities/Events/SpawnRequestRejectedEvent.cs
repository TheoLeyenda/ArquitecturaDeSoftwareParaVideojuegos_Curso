using ImageCampus.ToolBox.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Controllers.Events
{
	public struct SpawnRequestRejectedEvent<EntityType> : IEvent
	{
		public string blueprintToSpawn;
		public Coordinate coordiateToSpawn;
		public string message;
		public void Assign(params object[] parameters)
		{
			blueprintToSpawn = (string)parameters[0];
			coordiateToSpawn = (Coordinate)parameters[1];

			if (parameters.Length > 2)
				message = (string)parameters[2];
		}

		public void Reset()
		{
			blueprintToSpawn = string.Empty;
			coordiateToSpawn = default(Coordinate);
			message = default(string);
		}
	}

	public struct SpawnRequestRejectedEvent : IEvent
	{
		public string blueprintToSpawn;
		public Coordinate coordinateToSpawn;
		public string entityTypeName;
		public string message;

		public void Assign(params object[] parameters)
		{
			blueprintToSpawn = (string)parameters[0];
			coordinateToSpawn = (Coordinate)parameters[1];
			entityTypeName = (string)parameters[2];
			if (parameters.Length > 3)
				message = (string)parameters[3];
		}

		public void Reset()
		{
			blueprintToSpawn = string.Empty;
			coordinateToSpawn = default(Coordinate);
			entityTypeName = default(string);
			message = default(string);
		}
	}
}
