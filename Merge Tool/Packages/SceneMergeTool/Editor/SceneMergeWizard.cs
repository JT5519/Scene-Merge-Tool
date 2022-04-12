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
	[Tooltip("Applies all prefab overrides in the modifier scene, before merging. If false, then newly created prefab instances, in the modified scene during the merge, will not have their modifier scene overrides.")]
	public bool applyPrefabOverrides = true;
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
			modifierScene = EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(modifierSceneAsset), OpenSceneMode.Additive);
		if (!modifiedScene.IsValid())
			modifiedScene = EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(modifiedSceneAsset), OpenSceneMode.Additive);
		
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		SceneManager.SetActiveScene(modifiedScene);

		//setup for merging
		GameObject[] modifierRootObjects = modifierScene.GetRootGameObjects();
		GameObject[] modifiedRootObjects = modifiedScene.GetRootGameObjects();
		HashSet<GameObject> modifierSceneObjects = new HashSet<GameObject>(modifierRootObjects);

		//smart merge (is recursive)
		SmartMerge(modifierRootObjects,modifiedRootObjects,modifierSceneObjects);

		EditorSceneManager.MarkSceneDirty(modifiedScene);
	}
	private void SmartMerge(GameObject[] sourceArray, GameObject[] destArray, HashSet<GameObject> modifierSceneObjects,
														GameObject sourceParent = null, GameObject destinationParent = null)
    {
		
		//stopping condition
		if(destArray.Length == 0 && sourceParent && destinationParent)
        {
			//Adding children from source to destination
			foreach(GameObject child in sourceArray)
            {
				GameObject childClone = null;

				if (PrefabUtility.IsAnyPrefabInstanceRoot(child))
				{
					if(applyPrefabOverrides)
						PrefabUtility.ApplyPrefabInstance(child, InteractionMode.UserAction);
					GameObject objectSource = PrefabUtility.GetCorrespondingObjectFromOriginalSource(child);
					childClone = PrefabUtility.InstantiatePrefab(objectSource) as GameObject;
				}
				else
					childClone = GameObject.Instantiate(child);
				
				childClone.transform.SetParent(destinationParent.transform);
            }
			return;
        }
		else if(sourceArray.Length == 0 && sourceParent && destinationParent)
			return;

		//to track modified scene objects already mapped to a modifier scene object
		HashSet<GameObject> modifiedSceneObjects = new HashSet<GameObject>(destArray);
		
		//recursive iteration, can have devastating time complexity
		foreach(GameObject modified in destArray)
        {
			foreach(GameObject modifier in sourceArray)
            {
				//if current two objects are equal, and the modifier object hasnt already been deemed equal to another object
				if(CompareGameObjects(modifier,modified) && modifierSceneObjects.Contains(modifier) && modifiedSceneObjects.Contains(modified))
                {
					//Smart merge components
					SmartMergeComponents(modified, modifier);

					//create two new arrays of child game objects
					GameObject[] newSource = GenerateChildObjectArray(modifier);
					GameObject[] newDest = GenerateChildObjectArray(modified);
					
					//new hashset for modifier's child game objects
					HashSet<GameObject> newModifierSceneObjects = new HashSet<GameObject>(newSource);
                    
                    //recursive smart merge on child objects
                    SmartMerge(newSource,newDest,newModifierSceneObjects, modifier, modified);

					//modified and modifier objects have been handled, remove them from respective sets
					modifierSceneObjects.Remove(modifier);
					modifiedSceneObjects.Remove(modified);
				}
            }
        }

		//add remaining objects from modifier scene
		foreach (GameObject remainingObjects in modifierSceneObjects)
		{
			GameObject objClone = null;
			if (PrefabUtility.IsAnyPrefabInstanceRoot(remainingObjects))
			{
				if (applyPrefabOverrides)
					PrefabUtility.ApplyPrefabInstance(remainingObjects, InteractionMode.UserAction);

				GameObject objectSource = PrefabUtility.GetCorrespondingObjectFromOriginalSource(remainingObjects);
				objClone = PrefabUtility.InstantiatePrefab(objectSource) as GameObject;
			}
			else
				objClone = GameObject.Instantiate(remainingObjects);

			if (destinationParent)
				objClone.transform.SetParent(destinationParent.transform);
		}
	}
	private static bool CompareGameObjects(GameObject obj1, GameObject obj2)
    {
		//two objects are equal if they have the same Name, Layer, Tag, and static status
		if (obj1.isStatic == obj2.isStatic && obj1.name == obj2.name && obj1.layer == obj2.layer && obj1.tag == obj2.tag && ComparePrefabness(obj1,obj2))
			return true;

		return false;
    }
	private static bool ComparePrefabness(GameObject obj1, GameObject obj2)
    {
		if (PrefabUtility.IsOutermostPrefabInstanceRoot(obj1) == PrefabUtility.IsOutermostPrefabInstanceRoot(obj1)
		 && PrefabUtility.IsAnyPrefabInstanceRoot(obj1) == PrefabUtility.IsAnyPrefabInstanceRoot(obj2)
	   	 && PrefabUtility.IsPartOfPrefabInstance(obj1) == PrefabUtility.IsPartOfPrefabInstance(obj2))
			return true;
		
		return false;
    }
	private GameObject[] GenerateChildObjectArray(GameObject obj)
    {
		GameObject[] childArray = new GameObject[obj.transform.childCount];

		for (int i = 0; i < childArray.Length; i++)
			childArray[i] = obj.transform.GetChild(i).gameObject;

		return childArray;
	}
	private void SmartMergeComponents(GameObject modified, GameObject modifier)
    {
		HashSet<Component> modifierComponents = new HashSet<Component>(modifier.GetComponents<Component>());
		HashSet<Component> modifiedComponents = new HashSet<Component>(modified.GetComponents<Component>());

		foreach (Component modifiedComponent in modified.GetComponents<Component>())
		{
			foreach (Component modifierComponent in modifier.GetComponents<Component>())
			{
				if (modifierComponent.GetType() == modifiedComponent.GetType() && modifierComponents.Contains(modifierComponent) && modifiedComponents.Contains(modifiedComponent))
				{
					//TODO: Conflict handling of the two components

					//remove from component list for one-to-one matching
					modifierComponents.Remove(modifierComponent);
					modifiedComponents.Remove(modifiedComponent);
				}
			}
		}

		//Add unique components in modifier scene to modified scene
		foreach (Component remainingComponent in modifierComponents)
		{
			modified.AddComponent(remainingComponent.GetType());

			//TODO: component specific value modifications 
		}
	}
}