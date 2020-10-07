# Quick Start Guide

Thank you for using WIMVR! This chapter explains how to get started as quickly as possible.
After importing the plugin, it will present the welcome screen. if it doesn't appear, you can open it by selecting

`Window > WIMVR > Welcome Window` from the menu bar.

\begin{figure}[!h]
    \centering
    \includegraphics[width=\textwidth]{content/res/WelcomeWindow2.png}
    \caption{Welcome window.}
    \label{fig:WelcomeWindow}
\end{figure}

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

Before using this plugin, have a look at these steps to make sure it's set up properly. Once everything is set up, you are ready to go. Check out the example scene to give it a try right away. You can select the example scene from the welcome window. Alternatively, you can find it in your Assets folder at `Assets/WIMVR/Examples/SimpleExample`. If you want to use the World-in-Miniature in you own scene, have a look at [Chapter 1.6 'Configure Scene'](#configure-scene).

### Requirements

- This plugin has so far only been tested with an **Oculus Quest**.
- **Universal Render Pipeline (URP)**. Use the package manager to install the URP. Please follow [these instructions](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@8.2/manual/InstallURPIntoAProject.html) if you are trying to install the URP into an existing project.
- Additionally, these packages must be installed from the package manager:
  - **Input System**
  - **XR Interaction Toolkit**
  - **XR Plugin Management**
  - The **XR plugin** for your respective device, e.g. the **Oculus XR Plugin**

### Project Setup

<!-- TODO: Add screenshots  -->

Make sure you've followed all of the steps below.

- Switch the platform to Android in the `Build Settings`
- Make sure to use the OpenGLES3 graphics API (`Project Settings ->Player -> Other Settings`)
- Make sure you properly set up the [Universal Render Pipeline (URP)](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@8.2/manual/InstallURPIntoAProject.html) and [upgraded your shaders](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@8.2/manual/upgrading-your-shaders.html).
<!-- - If you did not set up the Universal Render Pipeline (URP):
  - Convert materials to Universal Render Pipeline: Select `Edit -> Render Pipeline -> Universal Render Pipeline -> Upgrade Project Materials to Universal RP Materials` from the menu.
  - Assign the *UniversalRenderPipelineAsset* in the graphics settings (`Project Settings -> Graphics`) -->
<!-- FIXME: what about the Android manifest? -->
<!-- - Move `Assets/WIMVR/AndroidManifest.xml` to  
  `Assets/Plugins/Android/AndroidManifest.xml` -->
<!-- * Create Android manifest: Select `Oculus -> Tools -> Create store-compatible AndroidManifest.xml` from the menu.
* Edit Android manifest: Change line  
  ```
  <category android:name="android.intent.category.INFO"/>
  ```
  to  
  ```
  <category android:name="android.intent.category.LAUNCHER"/>
  ```. -->
<!-- FIXME: Add how to set up XR management -->

### Tags

To edit your project's tags and layers, select  ```Edit Layers``` from the ```Layers``` dropdown (see Fig. \ref{fig:EditTagsAndLayers}). You can also edit the tags in your project settings.

\begin{figure}[!h]
    \centering
    \includegraphics[width=.5\textwidth]{content/res/EditTagsAndLayers.png}
    \caption{To edit your project's tags and layers, select  `Edit Layers` from the `Layers` dropdown.}
    \label{fig:EditTagsAndLayers}
\end{figure}

Make sure the following tags exist (exact spelling matters):  

- ```WIM```
- ```Level```
- ```WIM Level Old```
- ```HandL```
- ```HandR```
- ```ThumbR```
- ```ThumbL```
- ```IndexR```
- ```IndexL```
  
Additional tags for Pro version (exact spelling matters):

- ```PreviewCamera```
- ```PreviewScreen```
- ```Box Mask```
- ```Cylinder Mask```
- ```Spotlight Mask```

### Layers

To edit your project's tags and layers, select  ```Edit Layers``` from the ```Layers``` dropdown (see Fig. \ref{fig:EditTagsAndLayers}). You can also edit the tags in your project settings.

Make sure the following layers exist (exact spelling matters):

- ```WIM```
- ```Hands```
- ```Player```
- ```Fingers```
- ```PinchGrabbable```

### Layer Collision Matrix

Also, set up the layer collision matrix under `Project Settings -> Physics` so that the 'WIM' layer doesn't collide with any other layer except the 'Hands' layer (see Fig. \ref{fig:CollisionMatrix}).

\begin{figure}[!h]
    \centering
    \includegraphics[width=\textwidth]{content/res/LayerCollisionMatrix.png}
    \caption{The Layer Collision Matrix. Make sure to replicate this exact settings.}
    \label{fig:CollisionMatrix}
\end{figure}

<!-- ## Video Tutorial -->
<!-- TODO: Insert tutorial URL  -->

<!-- Should you prefer to watch a video click [here (coming soon)](https://www.youtube.com/channel/UC0mxcocqWRJ30-0T9usPHWA). -->

<!-- Coming soon. -->

## Configure Scene

1. All gameobjects that are part of your level must be nested under an empty gameobject.
   - Tag this empty gameobject as 'Level'.
<!-- FIXME: -->
1. Player Controller **[DEPRECATED]**
   <!-- * Add *OVRPlayerController Variant* prefab to scene (`WIMVR/Prefabs/Player/OVRPlayerController Variant`)
   * Add both hand prefabs to scene (`WIMVR/Prefabs/Player/CustomHandLeft Variant` and `WIMVR/Prefabs/Player/CustomHandRight Variant`)
   * Set the 'Parent Transform' property in the *OVR Grabber* inspector to the 'TrackingSpace' (child of 'OVRPlayerController') for both hands -->
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
1. Add miniature model
   - Add *Miniature Model* prefab to scene
   - Make sure both the tag and layer are set to 'WIM'
   - Configure miniature model to your preferences (see [Chapter 1.6 'Configure Miniature Model'](#configure-miniature-model))
   - Press 'Generate WIM' button

<!-- 4. Setup input manager
   * Add *Input Manager* prefab to scene
   * Select input mapping file
   * See Chapter 1.8 'Configure Input' to learn more -->

## Configure Miniature Model

To configure the WIM, select the WIM gameobject in the scene. All settings are displayed in the *Miniature Model* component inspector.
The configuration is stored in a *WIMConfiguration* asset, so you have to assign one.
Therefore, you can either select an existing *WIMConfiguration* (see Fig. \ref{fig:AssignWIMConfiguration}) or crate a new one using the create menu (see Fig. \ref{fig:CreateWIMConfiguration}).

\begin{figure}[!h]
    \centering
    \includegraphics[width=\textwidth]{content/res/SelectWIMConfiguration.png}
    \caption{Select an existing WIM configuration.}
    \label{fig:AssignWIMConfiguration}
\end{figure}

\begin{figure}[!h]
    \centering
    \includegraphics[width=\textwidth]{content/res/CreateWIMConfiguration.png}
    \caption{Create a new WIM configuration by selecting `Create -> WIM -> Configuration` either from the Assets menu or the right-click menu in the Project window.}
    \label{fig:CreateWIMConfiguration}
\end{figure}

To add or remove features, add or remove their respective scripts to the gameobject. See [Chapter 2 'Features'](#features) for a comprehensive list of all features. Some features might add additional settings to the *Miniature Model* component inspector.

These are the basic settings:

- *Player Representation*: The player's representation (prefab) in the miniature model. Used to indicate player's current position and orientation. Can be picked up and placed somewhere else if destination selection method is set to 'pickup'.
- *Destination Indicator*: Indicates the currently selected destination in the miniature model.
- *Scale Factor*: The scale factor applied to the miniature model on creation. Smaller numbers will result in a smaller model. A value of '1' would not downsize the miniature model at all.
- *WIM Level Offset*: Initial miniature model offset relative to this (parent) gameobject.
- *Expand Colliders*: Amount by which colliders of all miniature model objects should be extended to make it easier to grab.
- *Destination Always on the Ground*: If active, the destination will automatically snap to ground level. This protects the player from being teleported to a location in mid-air.
- *Destination Selection Method*: How players choose a destination. Pickup: pickup and place player representation using index and thumb. Double-tap destination indicator using index to confirm. Touch: touch destination using index and press button. Press button to confirm.
- *Double Tap Interval*: Maximum time between two taps. Will not detect a double-tap if time is exceeded. Double-tapping the destination indicator is used to confirm a destination and start the travel phase when the pickup destination selection method is selected.
- *Semi-Transparent*: See [Chapter 2.15](#semi-transparent)
- *Transparency*: See [Chapter 2.15](#semi-transparent)
- *WIM Spawn at Height*: Default height to spawn the miniature model at.
- *Player Height (in cm)*: The player's height. No Exact value required.
- *WIM Spawn Distance*: Specifies how far away from the player the miniature model should be spawned.
- *Detect Arm Length*: See [Chapter 2.3](#experimental-detect-arm-length)
- *Adapt miniature model Size to Player Height*: Automatically adapt the miniature model's size to the player's height. The effect will be minimal. Also use scale factor.

## Configure Input

<!-- FIXME: -->
[SECTION DEPRECATED]
