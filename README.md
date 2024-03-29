# Scene-Merge-Tool

v1.3.1-alpha is downloadable [here](https://github.com/JT5519/Scene-Merge-Tool/releases/tag/v1.3.1-alpha). It is a .zip that can be added to Unity using UPM (Unity Package Manager).

## Overview
This is a a tool that makes merging two scenes in Unity easy. The tools intended use case is to sit on a level above version control, where on an individual feature branch, multiple team member may work on sub-features parallelly. Merging these features is usually done through prefabs, but this tool eliminates the need for that unless necessary. It uses a recursive smart merge algorithm I built myself, that merges all objects in the two scenes, and recursively merges objects it deems as the same (based on certain parameters) with the recursion smartly merging the child objects as well, all the way down the hierarchy. 

## How it Works:
The tool maintains lists of root scene objects of both the "modifier" scene (the scene from which new objects are taken) and the "modified" scene (the scene into which the new objects are added). It then recursively compares objects in the two lists, where if it deems two objects to be the "same" in both scenes, it begins comparing their child objects. It goes all the way down the transform hierarchy, and fills in missing child objects in the modified scene from the modifier scene. It then checks for missing components between the two scene objects and makes use of reflection to duplicate any missing components. 
### A caveat: 
One drawback of this tool, is the devastating time complexity. It has an O(n^2) for the list comparisons. At each layer of the transform hierarchy, it compares lists of child objects with the same O(n^2) complexity. Hence, depending on the number of objects, and how layered the transform hierarchy of the scenes are, the time this algorithm takes can rise very quickly. This project was made as part of a graduate tools course, and given the time to develop the tool, the current state satisfied course requirements, but is not ideal for a release tool. 

## How to Install:
1.	Import the package into your Unity project using Unity’s package manager:
    1. Window -> Package Manager -> ‘+’ symbol -> Add package from disk -> Navigate to the extracted package folder and choose the package.json file. 
    2. Unity will then install the package into the current Unity project and the package will show up under the Packages folder as “Scene Merge Tool”
2.	Once the package is imported, a new menu item will show up called “Merge Tool”. On expanding it, it has a single option called “Load Merge Wizard”. Clicking this opens the merge tool window.


## How to Use:
1. Parameters:
   1. Modifier Scene: The scene that merges into the Modified scene.
   2. Modified Scene: The scene that is merged into by the Modifier scene.
2. Dragging the the approriate 'SceneAssets' to both parameters, validates the Scriptable Wizard and enables the "Merge" button which can now be clicked to have these two scenes merge.


## Future Updates:
1. Component conflicts handling
2. Instantiated object's name cleanup, i.e., removing the extra "(Clone)" or "(1)" that appends to the end of instantiated/duplicated objects
 
