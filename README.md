# Scene-Merge-Tool

v1.2.0 is downloadable [here](https://github.com/JT5519/Scene-Merge-Tool/releases). It is a .zip that can be added to Unity using UPM (Unity Package Manager).

## Overview
This is a a tool that makes merging two scenes in Unity easy. The tools intended use case is to sit on a level above version control, where on an individual feature branch, multiple team member may work on sub-features parallelly. Merging these features is usually done through prefabs, but this tool eliminates the need for that unless necessary. It uses a recursive smart merge algorithm I built myself, that merges all objects in the two scenes, and recursively merges objects it deems as the same (based on certain parameters) with the recursion smartly merging the child objects as well, all the way down the hierarchy. 



## How to Install:
1.	Import the package into your Unity project using Unity’s package manager:
    1. Window -> Package Manager -> ‘+’ symbol -> Add package from disk -> Navigate to the extracted package folder and choose the package.json file. 
    2. Unity will then install the package into the current Unity project and the package will show up under the Packages folder as “Scene Merge Tool”
2.	Once the package is imported, a new menu item will show up called “Merge Tool”. On expanding it, it has a single option called “Load Merge Wizard”. Clicking this opens the merge tool window.


## How to Use:
1. Parameters:
   1. Modifier Scene: The scene that merges into the Modified scene.
   2. Modified Scene: The scene that is merged into by the Modifier scene.
   3. Apply Prefab Overrides: Checking this boolean ensures that all overrides to prefabs in the Modifier scene are applied to their respective prefab assets before merging begins. This ensures newly instantiated prefab instances in the modified scene will have those changes.
2. Dragging the the approriate 'SceneAssets' to both parameters, validates the Scriptable Wizard and enables the "Merge" button which can now be clicked to have these two scenes merge.


## Future Updates:
QoS and QoL updates.
 
