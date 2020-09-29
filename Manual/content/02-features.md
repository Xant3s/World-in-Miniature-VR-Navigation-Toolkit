
<!-- First line has to be empty. -->
# Features

This chapter provides a comprehensive overview of all features. Features are subject to change. Especially experimental features might be changed or discarded. To add a feature, add the respective script to the miniature model gameobejct. Some features can be configured. The configuration is stored in its own config file (very much like the miniature model configuration itself).
If a feature can be configured, its settings will be displayed in the miniature model inspector among the other settings. If the feature requires player input, additional settings will also be added to the input manager.
To remove a feature, remove the respective script from the miniature model gameobject. Settings of disabled features will not be displayed until these features are activated again.

<!-- TODO: ingame vs in game?  -->
<!-- TODO: smaller images? -->
<!-- TODO: ref paper -->

<!-- ## Grab/Carry Miniature Model

**Description**  

Allows the player to pickup the entire miniature model and carry it.

**How to Use Ingame ** 

The player can press the specified grab button on either controller.

**Setup**  

Requires no setup. This is one of the basic features.

**Configuration**

No configuration required.

**Input**

Configure 'Right Grab Button' and 'Left Grab Button' in the input manager.

**Note**

This feature is always enabled by default. To disable it, don't assign any key mapping to the 'Right Grab Button' and 'Left Grab Button'. -->
<!-- If you would want to disable it, you'd have to remove the dependency to *OVRGrabbable* in the *MiniatureModel* script. Then, you could remove the *OVRGrabbable* component from the miniature model. -->
<!-- TODO: second one would be better because mapping required eg by scaling -->


<!-- ## Destination Selection Pickup

![Destination Selection Pickup](content/res/PickupSmall.png)

**Description**  
One of two alternative destination selection methods. Allows the player to pickup the representation of himself/herself and place it somewhere else in the miniature model. Thereby, a travel destination is selected. The destination has to be confirmed to start the travel phase. This is the recommended destination selection method.

**How to Use Ingame**   -->
<!-- The player has to pinch index and thumb to pickup the red player representation in the WIM. A blue destination representation will be picked up. The red player representation stays in place as the player does not move. The blue destination representation can be placed in the miniature model to indicate the desired target position. Once placed, the player can pickup the blue destination indicator and place it somewhere else. Alternatively, the player can pickup the  -->
<!-- The user can instantiate a destination indicator, identical
to the one used with direct selection, by grabbing the red user representation in the WIM. Therefore, the user must pinch the index finger and thumb. The destination indicator will be picked up, while the user’s representation stays in place. The user can then place the destination indicator anywhere in the WIM. To change the orientation, the user must turn his or her hand accordingly. The destination indicator can be dropped and picked up again to change the selected destination or orientation. Alternatively, the user can pull another destination indicator out of the red user  representation in the WIM. In this case, the old destination indicator will disappear. To confirm the destination, the destination indicator in the WIM must be double-tapped.


**Setup**  
Set 'Destination Selection Method' to 'Pickup' in miniature model inspector to enable. Requires no additional setup. This is one of the basic features.

**Configuration**  
No configuration required.

**Input**  
Configure 'Pickup Thumb Button' and 'Pickup Index Button' in the input manager. Additionally, the 'Pickup Thumb Button (Touch)' can be configured. It should be configured to the same mapping as 'Pickup Thumb Button'. This is optional, but strongly advised. Configuring this additional button allows to detect when the player no longer touches the thumb button. This will yield much better results and prevents some visual discrepancies (e.g. the player still holding the object only with the index finger). Currently, only one hand (the right hand by default) can be used at a time. -->


<!-- ## Destination Selection Touch

![Destination Selection Touch](content/res/SelectDestinationSmall.png)

**Description**  
One of two alternative destination selection methods. Allows the player to touch a location in the miniature model and mark it as destination by pressing a button. The destination can be confirmed by pressing another button.

**How to Use Ingame**
The player has to touch the desired destination with his index finger in the miniature model. To confirm the destination, a button has to be pressed. A thumbstick can be used to change the desired orientation. Another button has to be pressed to start the travel.

**Setup**  
Add the *DestinationSelectionTouch* script to the miniature model. Set 'Destination Selection Method' to 'Touch' in miniature model inspector to enable.

**Configuration**  
No configuration required.

**Input**  
Configure 'Destination Selection Button', 'Confirm Travel Button', and 'Destination Rotation Thumbstick' in the input manager. -->


<!-- ## Respawn -->

<!-- **Description**  
Allows the user to respawn the miniature model. This is especially useful if the user left the miniature model somewhere in the VE. It will be spawned at the user’s
position using an offset specified beforehand.

**How to Use Ingame**  
The user has to press the respawn button.

**Setup**  
Requires no setup. This is one of the basic features.

**Configuration**  
No configuration required.

**Input**  
Assign 'Respawn Button' in the input manager.

**Note**  
This feature is always enabled by default. To disable it, don't assign any key mapping to the 'Respawn Button'. -->


<!-- ## Semi-Transparent  

![Occlusion Handling: Semi-Transparent](content/res/TransparentWIM.png)

**Description**  
Draws the miniature model semi-transparently to mitigate issues with occlusion.

**How to Use Ingame**  
 -

**Setup**  
Check 'Semi-Transparent' in the miniature model inspector to enable.

**Configuration**  
Use 'Transparency' slider to choose transparency.

**Input**  
 - -->


<!-- ## Experimental: Detect Arm Length

**Description**  
Detects the player's arm length at the start of the game. The detected arm length will be used instead of the spawn distance when respawning the miniature model.

**How to Use Ingame**  
At the start of the game, the player has to extend the arm and press the confirm travel button. This has to be repeated every time the game is started. There is currently no visual feedback.

**Setup**  
Add the *DetectArmLength* script to the miniature model gameobject. Check 'Auto Detect Arm Length' in the miniature model inspector to enable.

**Configuration**  
 -

**Input**  
The 'Confirm Travel Button' has to be set in the input manager. -->


<!-- ## Scaling (Pro)

**Description**  
Allows the player to resize the miniature model at runtime.

**How to Use Ingame**  
To resize the miniature model, the player can grab it using both hands.
The WIM can be scaled up by moving the hands apart from each
other and scaled-down by bringing the hands closer together.

**Setup**  
Add the *Scaling* scrip to the miniature model gameobject. Check 'Allow WIM Scaling' to enable.

**Configuration**   -->
<!-- TODO: -->

<!-- * When scaling the miniature model up or down, the player changes the the scale factor. The scale factor represents the miniature model's current size.
* Change 'Min Scale Factor' to set the smallest allowed scale factor. This should be a positive number.
* Change 'Max Scale Factor' to set the largest allowed scale factor. A scale factor of '1' would result in the miniature model being a 1:1 copy of the full-sized level, thus also full sized.
* 'Scale Step' is used to determine how fast the scale factor can change. Small numbers will only allow small changes, large number will allow much faster scaling.
* 'Inter Hand Distance Delta Threshold': Ignore inter hand distance deltas below this threshold for scaling. Important so that not every tiny hand jitter will trigger scaling.

**Input**  
Configure “Right Grab Button” and “Left Grab Button” in the input manager. -->


<!-- ## Scrolling  (Pro) -->

<!-- TODO: image -->
<!-- **Description**  
When enabled, only an excerpt of the miniature model will be visible. Allows to player to decide which part of the miniature model should be visible at runtime. Especially useful when the level is very large and thus the miniature model would be very large, too.

**How to Use Ingame**  
There are two possible scrolling modes. The automatic  scrolling mode is useful in combination with the destination selection method 'touch', which is described in Section 2.3. If a destination is selected, the visual excerpt will
be centered on the destination. The visual excerpt will be centered on the user representation otherwise. Using the automatic scrolling mode requires no additional user interaction.  

The manual scrolling mode is useful in combination with the 'pickup' destination selection method described in Section
2.2. Using the manual scrolling mode, the user can use a thumbstick to move the visual excerpt of the miniature model.

**Setup**  
Add the *Scrolling* script to the miniature model gameobject. Check 'Allow WIM Scrolling' to enable.

**Configuration**   -->
<!-- TODO: -->

<!-- * 'Allow WIM Scrolling': whether this feature is enabled
* 'Active Area Bounds': Bounds of the box collider the player can use to grab the miniature model.
* 'Auto Scroll': Whether scrolling should be automated, so that the destination indicator is centered. If no destination is selected, the player is centered. If disabled, the player can scroll manually using thumbsticks.
* 'Scroll Speed': The default scroll speed. Only available when 'Auto Scroll' is enabled.
* 'Allow Vertical Scrolling': Whether vertical scrolling should be allowed. This required a second thumbstick. Only available when 'Auto Scroll' is disabled.

**Input**  
When 'Auto Scroll' is enabled, no input is required. Otherwise, configure 'Scrolling Axis' in the input manager. Also configure 'Vertical Scrolling Axis' in the input manager, if 'Allow Vertical Scrolling' is enabled. -->


<!-- ## Occlusion Handling: Cutout View  (Pro) -->

<!-- TODO: image -->
<!-- **Description**  
Geometries which are within proximity of an area
defined by a cylinder are clipped. As opposed to the “melting
walls” technique, the cylinder is attached to the user’s virtual head in the virtual environemnt.

**How to Use Ingame**  
The player doesn't need to do anything to activate this feature. It will be automatically used when the player's head is close to the miniature model.

**Setup**  
Add the *OcclusionHandling* script to the miniature model gameobject. Select 'Cutout View' as 'Occlusion Handling Strategy' in the miniature model inspector. -->

<!-- **Configuration**   -->
<!-- TODO: -->

<!-- * 'Cutout Range': Height of the view cone hiding the miniature model. The pointy end origins at the player's eyes.
* 'Cutout Angle': Angle of the view cone hiding the miniature model.
* 'Show Cutout Light': Whether the view cone should be visualized by a spotlight.
* 'Cutout Light Color': The color of the spotlight. Only available if 'Show Cutout Light' is enabled.

**Input**  
 - -->

<!-- ## Occlusion Handling: Melting Walls (Pro)

![Occlusion Handling Strategy: Melting Walls](content/res/MeltingWalls.png) -->

<!-- TODO: -->
<!-- **Description**  
As soon as the player's hand approaches the WIM, those parts of the WIM in close proximity to the hand start to fade. When the hand is moved elsewhere, the entire WIM is visible again.

**How to Use Ingame**  
The player doesn't need to do anything to activate this feature. It will be automatically used when the player's hand is close to the miniature model.

**Setup**  
Add the *OcclusionHandling* script to the miniature model gameobject. Select 'Melting Walls' as 'Occlusion Handling Strategy' in the miniature model inspector. -->

<!-- **Configuration**   -->
<!-- TODO: -->
<!-- 
* 'Melt Radius': Radius around the player's arm in which the miniature model is hidden.
* 'Melt Height': Distance parallel to the player's arm in which the miniature model is hidden.

**Input**  
 - -->


<!-- ## Preview Screen (Pro)

![Orientation Aid: Preview Screen](content/res/PreviewScreen.png)

**Description**  
The preview screen is a 2D panel that floats
in the VE and always faces the user. On the preview screen, the user can see a live preview of what the currently specified destination looks like, i.e. a continuously updated image showing exactly what the user will see once translated and rotated to the specified location and orientation. There are two different preview screen modes.

**How to Use Ingame**  
When the automatic mode is active, the WIM will be automatically displayed while a destination is selected. The preview screen will float above the WIM using a consistent offset. No user input is required.

The pickup mode works very similarly to the pickup destination selection method described in section 2.2. The destination indicator’s view frustum is displayed in the miniature model. The user can grab the view frustum by pinching the index finger and thumb. The preview screen will become visible and attached to the user’s hand. Afterwards, the user can position the preview screen anywhere in the VE. As soon as the preview screen is dropped, it will retain its position relative to the WIM. The preview screen can be closed by pressing a Close button which is displayed
in the upper right corner of the preview screen.

**Setup**  
Add *PreviewScreen* script to miniature model gameobject. Check 'Show Preview Screen' to enable.

**Configuration**  

* 'Show Preview Screen': Whether this feature is enabled.
* 'Auto Position Preview Screen': Whether the preview screen should be automatically opened and placed at a fixed position above the miniature model. Otherwise, the player has to grab the destination indicator's view cone using index and thumb and place the preview screen somewhere.

**Input**  
When the automatic mode is active, no input needs to be configured. Otherwise, configure 'Pickup Index Button', 'Pickup Thumb Button' and (optionally) 'Pickup Thumb Button (touch)'. -->


<!-- ## Travel Preview Animation (Pro)

![Orientation Aid: Travel Preview Animation](content/res/TravelPreviewAnimation.png)

**Description**  
The travel preview animation adds a visual path between the user's current position and selected destination in the miniature model. A semi-transparent user representation is animated to move along the path, thus providing a preview of the travel phase.

**How to Use Ingame**  
Will be displayed automatically.

**Setup**  
Add the *TravelPreviewAnimation* script to the gameobject. Check 'Travel Preview Animation' to enable.

**Configuration**  

* 'Travel Preview Animation': Whether this feature is enabled.
* 'Travel Preview Animation Speed': How long the travel preview animation should take in seconds. The animation is looped.

**Input**  
 - -->


<!-- ## Post-Travel Path Trace (Pro) -->
<!-- TODO: Better image -->

<!-- ![Orientation Aid: Path Trace](content/res/PathTrace.png)

**Description**  
A line from the previous position to the new position is displayed in the miniature model, visualizing the locomotion that just took place. The path trace is faded-out over time, from the previous position towards the new one.

**How to Use Ingame**  
Will be displayed automatically.

**Setup**  
Add the *PathTrace* script to the miniature model gameobject. Check 'Post Travel Path Trace' to enable.

**Configuration**  

* 'Post Travel Path Trace': Whether this feature is enabled.
* 'Trace Duration': How long the path trace is visible in seconds.

**Input**  
 - -->


<!-- ## Respawn Dissolve/Resolve FX (Pro) -->
<!-- TODO: improve image (multiple images?) -->

<!-- ![Respawn Dissolve-Resolve Effect](content/res/Dissolve1.png)

**Description**  
Adds an dissolve-resolve effect to the miniature model on respawn.

**How to Use Ingame**  
Will be displayed automatically.

**Setup**  
Add *DissolveFX* scrip to the miniature model.

**Configuration**  
 -

**Input**  
 - -->


<!-- ## Experimental: Live Update (Pro)

**Description**  
Updates the miniature model whenever a change to the full-sized level is made. Works both in editor and at runtime. This is still experimental. Only changes to transform components and child hierarchy are detected. Currently, this is not very performant.

**How to Use Ingame**  
 -

**Setup**  
Check 'Live Update WIM (experimental)' in miniature model inspector.

**Configuration**  
 -

**Input**  
 - -->


<!-- ## Experimental: Distance Grabbing (Pro) -->

<!-- TODO: Add image -->

<!-- **Description**  
Allows the player to point at certain objects and grab them from a distance. -->

<!-- **How to Use Ingame**  
Aim at target using the laser pointer. Hold grab button to start pulling the object. -->

<!-- **Setup**  

* Add *DistanceGrabbable* to every gameobject which should be grabbable from a distance, e.g. the miniature model gameobject.
* Replace 'CustomHandsLeft Variant' and 'CustomHandsRight Variant' prefabs with the respective pro versions: 'CustomHandsLeft Variant (Pro)' and 'CustomHandsRight Variant (Pro)'.

**Configuration**  
To configure the laser pointer find the 'AimAssist' child object of the respective hand. Modify the line component to change the visual appearance of the laser pointer. Use the *AimAssist* inspector to change additional settings:

* 'Hand': The hand this script is attached to.
* 'Length': The length of the laser pointer.

Also, these settings can be modified using the *DistanceGrabber* inspector which is attached to the respective hand:

* 'Hand': Hand this script is attached to.
* 'Start': Start point of the laser pointer. This should be set to the 'Aim Assist' child object.
* 'Required Distance to WIM': Distance grabbing will be disabled if within specified distance to miniature model. 
* 'Snap Speed': Specifies how fast objects are pulled towards hand.
* 'Min Distance': Stop pulling objects that are within specified distance to hand.
* 'Disable While in WIM': Automatically disable while this hand is touching the miniature model.


**Input**  
Configure 'Left Grab Button' and 'Right Grab Button' in the input manager. -->
