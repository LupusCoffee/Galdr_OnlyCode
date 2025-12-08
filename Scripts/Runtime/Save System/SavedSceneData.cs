using System;
using System.Collections.Generic;
using SaveSystem;

[Serializable]
public class SavedSceneData
{
	public int SceneIndex;

	public bool IsCompleted;

	public bool CurrentScene;
	
	public Dictionary<int, TransformSaveData> SavedTransforms = new();
	
	public SavedSceneData()
	{
		
	}
	
	public SavedSceneData(int sceneIndex)
	{
		SceneIndex = sceneIndex;
	}
	
	public void AddTransform(int id, TransformSaveData data)
	{
		SavedTransforms[id] = data;
	}

	public void Update(TransformSaveData transformSaveData, string transformId)
	{
		SavedTransforms[transformId.GetHashCode()] = transformSaveData;
	}

	public bool TryLoad(string transformId, out TransformSaveData transformSaveData)
	{
		return SavedTransforms.TryGetValue(transformId.GetHashCode(), out transformSaveData);
	}

	public bool Contains(string transformId)
	{
		return SavedTransforms.ContainsKey(transformId.GetHashCode());
	}
	
	public void Complete() => IsCompleted = true;
}