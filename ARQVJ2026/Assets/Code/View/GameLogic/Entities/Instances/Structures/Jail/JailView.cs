using System;
using System.Collections.Generic;
using UnityEngine;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Math;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Entities
{
    [ViewOf(typeof(Jail))]
    internal sealed class JailView : StrcutureView
    {
        private List<GameObject> perimeterInstances;

        public override Type ArchitectureEntityType => typeof(Jail);

        public override void Init(params object[] parameters)
        {
            base.Init(parameters);
            perimeterInstances = new List<GameObject>();
            GameObject perimeterPrefab = (GameObject)parameters[0];
            int spriteSortingOrder = (int)parameters[1];
            foreach (Point perimeterPoint in ArchitectureEntity.coordinate.Perimeter)
            {
                GameObject instance = Instantiate(perimeterPrefab,
                                                  GameScene.PointToWorld(perimeterPoint), 
                                                  Quaternion.identity, 
                                                  transform);
               
                if(instance.TryGetComponent(out SpriteRenderer spriteRenderer))
                    spriteRenderer.sortingOrder = spriteSortingOrder;

                perimeterInstances.Add(instance);
            }
        }

    }

}
