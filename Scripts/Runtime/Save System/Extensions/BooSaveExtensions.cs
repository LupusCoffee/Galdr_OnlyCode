// Created by Martin M

using UnityEngine;

namespace SaveSystem.Extensions
{
	public static class BooSaveExtensions
	{
		// public static void SaveTransform(this SavedTransform transform,
		// 	TransformSaveKind kind = TransformSaveKind.Position)
		// {
		// 	BooSave.Shared.Update(new TransformSaveData(transform.transform, kind),
		// 		transform.Id);
		// 	BooSave.Shared.Save();
		// }

		public static void UpdateSaveTransform(this SavedTransform transform,
			SavedSceneData sceneData,
			TransformSaveKind kind = TransformSaveKind.Position)
		{
			sceneData.Update(new TransformSaveData(transform.transform, kind),
				transform.Id);
		}

		public static void LoadTransform(this SavedTransform transform, SavedSceneData sceneData)
		{
			if (sceneData.TryLoad(transform.Id, out TransformSaveData data))
			{
				transform.transform.position = data.Position;
				transform.transform.rotation = data.Rotation;
				// transform.transform.localScale = data.Scale;
			}
			else
			{
				Debug.LogWarning($"Failed to load transform data for {transform.Id}\n" +
				                 $"May not exist in the save file yet or was not saved");
			}
		}

		public static void LoadTransform(this SavedTransform transform, TransformSaveData saveData)
		{
			transform.transform.position = saveData.Position;
			transform.transform.rotation = saveData.Rotation;
			// transform.transform.localScale = saveData.Scale;
		}
	}
}