
\pagebreak

## Experimental: Distance Grabbing

<!-- TODO: Add image -->

**Description**  
Allows the player to point at certain objects (including the WIM) and grab them from a distance.

**How to Use in Game**  
Aim at target using the laser pointer. Hold grab button to start pulling the object.

**Setup**  

- Add *DistanceGrabbable* to every gameobject which should be grabbable from a distance, e.g. the miniature model gameobject
- Add the *DistanceGrabbing* script to the miniature model gameobject
- Enable 'Distance Grabbing' in the *MiniatureModel* component inspector

**Configuration**  
To configure the laser pointer, find the 'AimAssist' prefab in the ```WIMVR/Prefabs/Resources``` directory. Modify the line component to change the visual appearance of the laser pointer. Use the *AimAssist* inspector to change additional settings:

- *Hand*: The hand this script is attached to
- *Length*: The length of the laser pointer

Also, these settings can be modified at runtime using the *DistanceGrabber* which is attached to the respective hand:

- *Hand*: Hand this script is attached to
- *Start*: Start point of the laser pointer. This should be set to the 'Aim Assist' child object
- *Required Distance to WIM*: Distance grabbing will be disabled if within specified distance to miniature model
- *Snap Speed*: Specifies how fast objects are pulled towards hand
- *Min Distance*: Stop pulling objects that are within specified distance to hand
- *Disable While in WIM*: Automatically disable while this hand is touching the miniature model

**Input Mappings**  
 -
