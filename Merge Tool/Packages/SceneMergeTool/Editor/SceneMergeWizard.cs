using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SceneMergeWizard : ScriptableWizard
{
	[Tooltip("Additvely merges into the \"Modified\" scene.")]
	public SceneAsset modifierSceneAsset;
	[Tooltip("Additvely merged into by the \"Modifier\" scene.")]
	public SceneAsset modifiedSceneAsset;

	[MenuItem("Merge Tool/Load Merge Wizard...")]
	static void Merge()
	{
		ScriptableWizard.DisplayWizard<SceneMergeWizard>("Scene Merger", "Merge");	
	}

    private void OnWizardUpdate()
    {
		if (!modifierSceneAsset|| !modifiedSceneAsset)
		{
			isValid = false;
			errorString = "Modifier and Modified scene assets need to be assigned before merging is possible!";
		}
		else
        {
			isValid = true;
			errorString = "";
        }
    }
    private void OnWizardCreate()
	{
		Scene modifierScene = SceneManager.GetSceneByName(modifierSceneAsset.name);
		Scene modifiedScene = SceneManager.GetSceneByName(modifiedSceneAsset.name);

		if (!modifierScene.IsValid())
		{
			modifierScene = EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(modifierSceneAsset), OpenSceneMode.Additive);

		}
		if (!modifiedScene.IsValid())
		{
			modifiedScene = EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(modifiedSceneAsset), OpenSceneMode.Additive);
		}
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		SceneManager.SetActiveScene(modifiedScene);

		//setup for merging
		GameObject[] modifierRootObjects = modifierScene.GetRootGameObjects();
		GameObject[] modifiedRootObjects = modifiedScene.GetRootGameObjects();
		HashSet<GameObject> modifierSceneObjects = new HashSet<GameObject>(modifierRootObjects);

		//smart merge (is recursive)
		SmartMerge(ref modifierRootObjects, ref modifiedRootObjects, ref modifierSceneObjects);

		EditorSceneManager.MarkSceneDirty(modifiedScene);
	}
	private static void SmartMerge(ref GameObject[] sourceArray, ref GameObject[] destArray, ref HashSet<GameObject> modifierSceneObjects,
														GameObject sourceParent = null, GameObject destinationParent = null)
    {
		
		//stopping condition
		if(destArray.Length == 0 && sourceParent && destinationParent)
        {
			//Adding children from source to destination
			foreach(GameObject child in sourceArray)
            {
				GameObject childClone = Object.Instantiate(child);
				childClone.transform.SetParent(destinationParent.transform);
            }

			/*
			 * Add extra components (if present) from modifier to modified
			*/

			return;
        }
		
		//recursive iteration, can have devastating time complexity
		foreach(GameObject modified in destArray)
        {
			foreach(GameObject modifier in sourceArray)
            {
				//if objects are equal
				if(CompareGameObjects(modifier,modified))
                {
					//create two new arrays of child game objects
					GameObject[] newSource = new GameObject[modifier.transform.childCount];
					GameObject[] newDest = new GameObject[modified.transform.childCount];				
					for (int i = 0;i<newSource.Length;i++)
                    {
						newSource[i] = modifier.transform.GetChild(i).gameObject;
                    }
					for (int i = 0; i < newDest.Length; i++)
					{
						newDest[i] = modified.transform.GetChild(i).gameObject;
					}
					
					//new hashset for modifier's child game objects
					HashSet<GameObject> newModifierSceneObjects = new HashSet<GameObject>(newSource);

					//recursive smart merge on child objects
					SmartMerge(ref newSource,ref newDest, ref newModifierSceneObjects, modifier, modified);

					//modifier object has been handled, remove it from the modifier objects set
					modifierSceneObjects.Remove(modifier);
				}
            }
        }

		//add remaining objects from modifier scene
		foreach (GameObject remainingObjects in modifierSceneObjects)
		{
			GameObject objClone = Object.Instantiate(remainingObjects);
			if(destinationParent)
            {
				objClone.transform.SetParent(destinationParent.transform);
            }
		}
	}
	private static bool CompareGameObjects(GameObject obj1, GameObject obj2)
    {
		//two objects are equal if they have the same Name, Layer, Tag, and static status
		if (obj1.isStatic == obj2.isStatic && obj1.name == obj2.name && obj1.layer == obj2.layer && obj1.tag == obj2.tag)
			return true;
		return false;
    }
}