using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Services;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZooArchitect.View.Data;

namespace ZooArchitect.View.Resources
{
    internal sealed class PrefabsRegistryView : IService
    {
        private BlueprintRegistry BlueprintRegistry => ServiceProvider.Instance.GetService<BlueprintRegistry>();
        private BlueprintBinder BlueprintBinder => ServiceProvider.Instance.GetService<BlueprintBinder>();

        public bool IsPersistance => false;

        private Dictionary<string, string> prefabPaths;
        private Dictionary<string, GameObject> prefabs;

        public PrefabsRegistryView()
        {
            prefabs = new Dictionary<string, GameObject>();
            prefabPaths = new Dictionary<string, string>();
            foreach (string id in BlueprintRegistry.BlueprintsOf(TableNamesView.PREFABS_VIEW_TABLE_NAME))
            {
                object prefabPath = new PrefabPath();
                BlueprintBinder.Apply(ref prefabPath, TableNamesView.PREFABS_VIEW_TABLE_NAME, id);
                prefabPaths.Add(((PrefabPath)prefabPath).architectureID, ((PrefabPath)prefabPath).PrefabResourcePath);
            }
        }

        public GameObject Get(string architecturID)
        {
            string resourcePath = prefabPaths[architecturID];

            if (prefabs.ContainsKey(resourcePath))
                return prefabs[resourcePath];

            GameObject prefab = UnityEngine.Resources.Load<GameObject>(resourcePath);
            prefabs.Add(resourcePath, prefab);
            return prefab;
        }

        private struct PrefabPath
        {
            [BlueprintParameter("Architecture ID")] public string architectureID;
            [BlueprintParameter("Prefab resource path")] private string prefabResourcePath;

            public string PrefabResourcePath => prefabResourcePath.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
