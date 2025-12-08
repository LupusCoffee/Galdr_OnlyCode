using UnityEditor;
using UnityEngine;

namespace Journal.Editor
{
	public class JournalsDatabaseAssetPostprocessor : AssetPostprocessor
	{
		public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
			string[] movedFromAssetPaths)
		{
			foreach (string assetPath in importedAssets)
			{
				JournalsDatabase asset = AssetDatabase.LoadAssetAtPath<JournalsDatabase>(assetPath);
				if (asset == null) continue;
				asset.AddJournalItemData();
				return;
			}
		}
	}
}