using System;
using System.Collections.Generic;
using UnityEngine;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Math;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Entities
{
	[ViewOf(typeof(LivingEntity))]
	internal abstract class LivingEntityView : EntityView
	{
		public override Type ArchitectureEntityType => typeof(LivingEntity);
		private LivingEntity ArchitectureLivingEntity => ArchitectureEntity as LivingEntity;

		private Queue<Coordinate> movementQueue;
		private bool ShouldMove => movementQueue.Count > 0;

		protected virtual float MoveSpeed => 3f;

		public override void Init()
		{
			movementQueue = new Queue<Coordinate>();
			base.Init();
		}

		public override void Move(Coordinate coordinate)
		{
			movementQueue.Enqueue(coordinate);
		}

		public override void Tick(float deltaTime)
		{
			if (!ShouldMove)
				return;

			Vector3 target = GameScene.CoordinateToWorld(movementQueue.Peek());

			transform.position = Vector3.MoveTowards(transform.position, target, MoveSpeed * deltaTime);

			if ((transform.position - target).sqrMagnitude <
				Vector3.kEpsilon * Vector3.kEpsilon)
			{
				transform.position = target;
				movementQueue.Dequeue();
			}
		}
	}
}
