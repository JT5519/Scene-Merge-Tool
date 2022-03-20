using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SceneMergeWizard : ScriptableWizard
{
	public SceneAsset sourceSceneAsset;
	public SceneAsset destinationSceneAsset;

	[MenuItem("Merge Tool/Load Merge Wizard...")]
	static void Merge()
	{
		ScriptableWizard.DisplayWizard<SceneMergeWizard>("Scene Merger", "Merge");	
	}

    private void OnWizardUpdate()
    {
		if (!sourceSceneAsset|| !destinationSceneAsset)
		{
			isValid = false;
			errorString = "Source and Destination scenes need to be set before merge is possible!";
		}
		else
        {
			isValid = true;
			errorString = "";
        }
    }
    private void OnWizardCreate()
	{
		Scene sourceScene = SceneManager.GetSceneByName(sourceSceneAsset.name);
		Scene destScene = SceneManager.GetSceneByName(destinationSceneAsset.name);

		if (!sourceScene.IsValid())
		{
			sourceScene = EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sourceSceneAsset), OpenSceneMode.Additive);

		}
		if (!destScene.IsValid())
		{
			destScene = EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(destinationSceneAsset), OpenSceneMode.Additive);
		}
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		EditorSceneManager.MergeScenes(sourceScene, destScene);
		EditorSceneManager.MarkSceneDirty(destScene);
	}
}