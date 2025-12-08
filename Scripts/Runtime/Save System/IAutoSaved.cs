// Created by Martin M
namespace SaveSystem
{
	public interface IAutoSaved
	{
		public bool LoadOnStart { get; set; }
		public void Save(bool force = false);
		public void UpdateSave(SavedSceneData sceneData, bool force = false);
		public void Load(SavedSceneData sceneData, bool force = false);
	}
}