using ImageCampus.ToolBox.Blueprints;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
	public sealed class Visitor : Human
	{
		public const string VALID_SPAWN_TIME_STEPS_KEY = "Valid spawn time steps";

		public const string ENTRANCE_PRICE = "Entrance price";
		public const string ENTRANCE_PRICE_RESOURCE_KEY = "Entrance price reource key";

		public const string ENTRANCE_COST = "Entrance cost";
		public const string ENTRANCE_COST_RESOURCE_KEY = "Entrance cost resource key";

		public const string NEEDED_INFTASTRUCTURES_TO_SPAWN = "Needed structures to spawn keys";

		[BlueprintParameter(VALID_SPAWN_TIME_STEPS_KEY)] private string[] validSpawnTimeSteps;
		[BlueprintParameter("Spawn delay")] private float spawnDelay;
		public float SpawnDelay => spawnDelay;

		[BlueprintParameter(ENTRANCE_PRICE)] private float entrancePrice;
		[BlueprintParameter(ENTRANCE_PRICE_RESOURCE_KEY)] private string entrancePriceReourceKey;

		[BlueprintParameter(ENTRANCE_COST)] private long entranceCost;
		[BlueprintParameter(ENTRANCE_COST_RESOURCE_KEY)] private string entranceCostReourceKey;

		[BlueprintParameter(NEEDED_INFTASTRUCTURES_TO_SPAWN)] private string neededInftastructuresToSpawn;

		private Visitor(uint ID, Coordinate coordinate) : base(ID, coordinate)
		{

		}
	}
}
