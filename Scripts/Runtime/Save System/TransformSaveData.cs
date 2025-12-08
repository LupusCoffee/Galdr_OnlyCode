// Created by Martin M

using System;
using UnityEngine;

namespace SaveSystem
{
	[Serializable]
	public class TransformSaveData
	{
		[SerializeField]
		public Vector3 Position;
		
		[SerializeField]
		public Quaternion Rotation;
		
		[SerializeField]
		public Vector3 Scale;

		[SerializeField]
		public uint SaveKind;
		
		public TransformSaveKind Kind => (TransformSaveKind) SaveKind;

		public TransformSaveData()
		{
			
		}
		
		public TransformSaveData(Transform transform, TransformSaveKind kind = TransformSaveKind.Position)
		{
			Position = transform.position;
			Rotation = transform.rotation;
			Scale = transform.localScale;
		}
	}
}