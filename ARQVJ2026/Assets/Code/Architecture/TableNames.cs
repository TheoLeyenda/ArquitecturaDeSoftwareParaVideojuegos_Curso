using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Controllers;
using ZooArchitect.Architecture.Entities;

namespace ZooArchitect.Architecture.Data
{
    public static class TableNames
    {
        public const string ANIMALS_TABLE_NAME = "Animals";
        public const string JAILS_TABLE_NAME = "Jails";
        public const string INFTRASTRUCTURE_TABLE_NAME = "Infrastructure";
        public const string WORKERS_TABLE_NAME = "Workers";
        public const string VISITORS_TABLE_NAME = "Visitors";

        public const string TILE_TYPES_TABLE_NAME = "Tile Types";
        public const string DAY_NIGHT_CYCLE_TABLE_NAME = "Day Night Cycle";

        public const string RESOURCES_TABLE_NAME = "Resources";
        public const string BUY_ITEMS_TABLE_NAME = "BuyItems";

        public const string CLEANING_SERVICE_TABLE_NAME = "Cleaning Service";
        public const string REPUTATION_SYSTEM_TABLE_NAME = "Reputation System";
        public const string SERVICES_LOGIC_TABLE_NAME = "Services logic";

        public const string TIME_SCALES_TABLE_NAME = "Time scales";

        public static readonly IReadOnlyDictionary<string, string> PURCHASABLE_ELEMENTS_MAPPING =
            new Dictionary<string, string>()
            {
                {RESOURCES_TABLE_NAME,nameof(BuyControllerArchitecture.PurchaseResource) },
                {WORKERS_TABLE_NAME,nameof(BuyControllerArchitecture.PurchaseEntity) },
            };

        internal static readonly IEnumerable<string> PURCHASABLE_ELEMENTS_TABLES = PURCHASABLE_ELEMENTS_MAPPING.Keys;

        public static readonly IReadOnlyDictionary<string, Type> ENTITY_TABLE_NAME_ENTITY_TYPE =
            new Dictionary<string, Type>()
            {
                {ANIMALS_TABLE_NAME,typeof(Animal) },
                {JAILS_TABLE_NAME,typeof(Jail) },
                {INFTRASTRUCTURE_TABLE_NAME,typeof(Infrastructure) },
                {WORKERS_TABLE_NAME,typeof(Worker) },
                {VISITORS_TABLE_NAME,typeof(Visitor) },
            };
    }
}
