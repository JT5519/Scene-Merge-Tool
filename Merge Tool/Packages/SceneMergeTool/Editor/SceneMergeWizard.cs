using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SceneMergeWizard : ScriptableWizard
{
	public SceneAsset destinationScene;
	public List<SceneAsset> scenesToCombine;


	[MenuItem("Merge Tool/Load Merge Wizard...")]
	static void Merge()
	{
		ScriptableWizard.DisplayWizard<SceneMergeWizard>("Scene Merging Wizard", "Merge", "Load Combined");	
	}

    private void OnWizardCreate()
	{
		
		Scene destScene = EditorSceneManager.GetSceneByName(destinationScene.name);
		foreach (SceneAsset item in scenesToCombine)
		{
			Scene nextScene = EditorSceneManager.GetSceneByName(item.name);
			EditorSceneManager.MergeScenes(nextScene, destScene);
		}
	}

	private void OnWizardOtherButton()
    {
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

		foreach (SceneAsset item in scenesToCombine)
			EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(item), OpenSceneMode.Additive);
	}
}