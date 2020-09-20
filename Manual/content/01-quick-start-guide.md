# Quick Start Guide

Thank you for using WIMVR! This chapter explains how to get started as quickly as possible. 
After importing the plugin, it will present the welcome screen. if it doesn't appear, you can open it by selecting

`Window > WIMVR > Welcome Window` from the menu bar.

![Welcome Window](content/res/WelcomeWindow.png)

## What is World-in-Miniature (WIM)?

Navigation is one of the most fundamental challenges
in Virtual Reality (VR). The world-in-miniature (WIM) metaphor allows players to travel in large-scale virtual environments (VEs) regardless of available physical space while maintaining a high-level overview of the VE. It relies on a hand-held, scaled-down duplicate of the entire VE, where the user’s current position is displayed, and an interface provided to introduce his/her next movements. This scaled-down version of the VE is called WIM or miniature model.


## What is WIMVR?

World-in-Miniature VR Navigation Toolkit (WIMVR) is a Unity plugin that integrates and extends state-of-the-art interaction tasks and
visualization concepts to overcome open conceptual gaps and to
provide a comprehensive practical solution for traveling in VR.


## Lite/Pro Version

There are two versions available in the Unity Asset Store.
The Lite version provides only the core world-in-miniature VR navigation features. To get the full-featured version, consider upgrading to Pro. Sections in this document that only apply to the Pro version of this plugin are marked as such.


## Setup

Before using this plugin, have a look at these steps to make sure it's set up properly. Once everything is set up, you are ready to go. Check out the example scene to give it a try right away. You can find it in your Assets folder at `Assets/WIMVR/Examples/SimpleExample`. If you want to use the World-in-Miniature in you own scene, have a look at [Chapter 1.6 'Configure Scene'](#configure-scene).


### Requirements

* This plugin currently only supports **Oculus Quest**.
* It has been tested with **Oculus Integration version 1.39**, other versions might require modifications. You can download it [here](https://developer.oculus.com/downloads/package/unity-integration-archive/1.39.0/).
* **Universal Render Pipeline (URP)**. Use the package manager to install URP.

**Note: Using with the XR Management package has not been tested.**

### Project Setup

<!-- TODO: Add screenshots -->

Make sure you've followed all of the steps below.

* Switch platform to Android in the `Build Settings`
* Remove Vulcan from the graphics APIs (`Project Settings ->Player -> Other Settings`)
* Convert materials to Universal Render Pipeline: Select `Edit -> Render Pipeline -> Universal Render Pipeline -> Upgrade Project Materials to Universal RP Materials` from the menu.
* Assign the *UniversalRenderPipelineAsset* in the graphics settings (`Project Settings -> Graphics`)
* Setup Oculus application ID: Select `Oculus -> Platform -> Edit Settings` from the menu. Enter your Oculus app ID in the 'Oculus Go/Quest or Gear VR' field. You can also insert a hyphen ('-') instead of an app ID.
* Move `Assets/WIMVR/AndroidManifest.xml` to  
  `Assets/Plugins/Android/AndroidManifest.xml`
<!-- * Create Android manifest: Select `Oculus -> Tools -> Create store-compatible AndroidManifest.xml` from the menu.
* Edit Android manifest: Change line  
  ```
  <category android:name="android.intent.category.INFO"/>
  ```
  to  
  ```
  <category android:name="android.intent.category.LAUNCHER"/>
  ```. -->
* Enable VR support: Go to `Project Settings -> Player -> XR Settings` and check 'Virtual Reality Supported'. Also add the Oculus SDK (see Fig.2).  
**Note: This will eventually be replaced by the new XR Management package.**
* Set 'Stereo Rendering Mode' to Single Pass (see Fig.2)

![XR Settings](content/res/XRSettings.png)


### Tags

Make sure the following tags exist (exact spelling matters).  
To add tags, go to ```Project Settings -> Tags and Layers```.  
* WIM  
* Level  
* WIM Level Old  
* HandL  
* HandR  
* ThumbR  
* IndexR  
* IndexL  
  
Additional tags for Pro version (exact spelling matters):  
* PreviewCamera  
* PreviewScreen  
* Box Mask  
* Cylinder Mask  
* Spotlight Mask  

### Layers

Make sure the following layers exist (exact spelling matters).  
To add layers, go to ```Project Settings -> Tags and Layers```.  
* WIM  
* Hands  
* Player  

### Layer Collision Matrix

Also, set up the layer collision matrix under `Project Settings -> Physics` so that the 'WIM' layer doesn't collide with any other layer except the 'Hands' layer (see Fig.3).

![Layer Collision Matrix](content/res/LayerCollisionMatrix.png)


<!-- ## Video Tutorial -->
<!-- TODO: Insert tutorial URL  -->

<!-- Should you prefer to watch a video click [here (coming soon)](https://www.youtube.com/channel/UC0mxcocqWRJ30-0T9usPHWA). -->

<!-- Coming soon. -->


## Configure Scene

1. All gameobjects that are part of your level must be nested under an empty gameobject.
   * Tag this empty gameobject as 'Level'.
2. Player Controller
   * Add *OVRPlayerController Variant* prefab to scene (`WIMVR/Prefabs/Player/OVRPlayerController Variant`)
   * Add both hand prefabs to scene (`WIMVR/Prefabs/Player/CustomHandLeft Variant` and `WIMVR/Prefabs/Player/CustomHandRight Variant`)
   * Set the 'Parent Transform' property in the *OVR Grabber* inspector to the 'TrackingSpace' (child of 'OVRPlayerController') for both hands
   <!-- * Add *OVRPlayerController* prefab to scene
   * Uncheck 'Enable Rotation' in the *OVRPlayerController* inspector
   * Add *CustomHandLeft* and *CustomHandRight* prefabs to scene
   * Tag the left hand as 'HandL' and the right hand as 'HandR'
   * Change the layer to 'Hands' for both hands. Also change children.
   * Set the 'Parent Transform' property in the *OVR Grabber* inspector to the 'TrackingSpace' (child of 'OVRPlayerController') for both hands
   * Setup right thumb:  
     * Search for 'b_r_thumb_ignore' in hierarchy
     * Set tag to 'ThumbR'
     * Add *Rigidbody* component. Disable 'Use Gravity' and enable 'Is kinematic'. 
     * Add *Sphere Collider* component. Check 'Is Trigger'. Set 'Center.X' to '-0.006' and 'Radius' to '0.01'. -->
3. Add miniature model
   * Add *Miniature Model* prefab to scene
   * Make sure both the tag and layer are set to 'WIM'
   * Configure miniature model to your preferences (see [Chapter 1.6 'Configure Miniature Model'](#configure-miniature-model))
   * Press 'Generate WIM' button
<!-- 4. Setup input manager
   * Add *Input Manager* prefab to scene
   * Select input mapping file
   * See Chapter 1.8 'Configure Input' to learn more -->

## Configure Miniature Model

The configuration is stored in an *WIMConfiguration* asset. Select an existing *WIMConfiguration* file or create a new one using the create menu. Therefore, right-click anywhere on the project window and select `Create -> WIM -> Configuration`.

To add or remove features, add or remove their respective scripts to the gameobject. See [Chapter 2 'Features'](#features) for a comprehensive list of all features. Some features might add additional settings to the *Miniature Model* inspector.

These are the basic settings:

* 'Player Representation': The player's representation (prefab) in the miniature model. Used to indicate player's current position and orientation. Can be picked up and placed somewhere else if destination selection method is set to 'pickup'.
* 'Destination Indicator': Indicates the currently selected destination in the miniature model.
* 'Scale Factor': The scale factor applied to the miniature model on creation. Smaller numbers will result in a smaller model. A value of '1' would not downsize the miniature model at all.
* 'WIM Level Offset': Initial miniature model offset relative to this (parent) gameobject.
* 'Expand Colliders': Amount by which colliders of all miniature model objects should be extended to make it easier to grab.
* 'Destination Always on the Ground': If active, the destination will automatically snap to ground level. This protects the player from being teleported to a location in mid-air.
* 'Destination Selection Method': How players choose a destination. Pickup: pickup and place player representation using index and thumb. Double-tap destination indicator using index to confirm. Touch: touch destination using index and press button. Press button to confirm.
* 'Double Tap Interval': Maximum time between two taps. Will not detect a double-tap if time is exceeded. Double-tapping the destination indicator is used to confirm a destination and start the travel phase when the pickup destination selection method is selected.
* 'Semi-Transparent': See [Chapter 2.15](#semi-transparent)
* 'Transparency': See [Chapter 2.15](#semi-transparent)
* 'WIM Spawn at Height': Default height to spawn the miniature model at.
* 'Player Height (in cm): The player's height. No Exact value required.
* 'WIM Spawn Distance': Specifies how far away from the player the miniature model should be spawned.
* 'Detect Arm Length': See [Chapter 2.3](#experimental-detect-arm-length)
* 'Adapt miniature model Size to Player Height': Automatically adapt the miniature model's size to the player's height. The effect will be minimal. Also use scale factor.


## Configure Input

<!-- ### Configure Input Using WIMVR Input Manager

**Note: This will eventually be replaced by the new Unity Input System.**  
**Note: The *Oculus Quest Mapper* will dynamically only display settings of enabled features. When you add a new feature which requires input, corresponding settings will become available in the *Oculus Quest Mapper* inspector. **

To change the input mapping locate the *WIM Input Manager* gameobject in the scene. All input mappings are stored to an *Input Mapping* asset. In the *Oculus Quest Mapper* inspector choose the *Input Mapping* you would like to use. You can use the provided standard mapping called 'OcclusionHandlingConfig' (find it by searching in project window) or create a new one. You can create a new *Input Mapping* file using the create menu. Therefore, right-click anywhere in the project window and choose `Create -> WIM -> Input Mapping`. Once you selected the *Input Mapping* you would like to use, you can change the individual button mappings using the *Oculus Quest Mapper* inspector. Your changes will be saved automatically. -->
