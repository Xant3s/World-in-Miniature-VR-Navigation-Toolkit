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

## Scientific Paper

In case you need a scientific reference, you can cite this [paper](https://dl.acm.org/doi/10.1145/3402942.3402994). You can also access if for free [here](https://www.researchgate.net/publication/344368828_An_Integrated_Design_of_World-in-Miniature_Navigation_in_Virtual_Reality). This paper is also useful if you wish to learn more about World-in-Miniature navigation.

BibTeX citation:

```
@inproceedings{10.1145/3402942.3402994,
author = {Truman, Samuel and von Mammen, Sebastian},
title = {An Integrated Design of World-in-Miniature Navigation 
in Virtual Reality},
year = {2020},
isbn = {9781450388078},
publisher = {Association for Computing Machinery},
address = {New York, NY, USA},
url = {https://doi.org/10.1145/3402942.3402994},
doi = {10.1145/3402942.3402994},
articleno = {69},
numpages = {9},
keywords = {world-in-miniature, virtual reality, navigation},
location = {Bugibba, Malta},
series = {FDG '20}
}
```


## Setup

Before using this plugin, have a look at these steps to make sure it's set up properly. Once everything is set up, you are ready to go. Check out the example scene to give it a try right away. You can select the example scene from the welcome window. Alternatively, you can find it in your Assets folder at `Assets/WIMVR/Examples/SimpleExample`. If you want to use the World-in-Miniature in you own scene, have a look at [Chapter 1.6 'Configure Scene'](#configure-scene).

### Requirements

\begin{itemize}
  \item This plugin has so far only been tested with an \textbf{Oculus Quest}.
  \item \textbf{Universal Render Pipeline (URP)}. Use the package manager to install the URP. Please follow [these instructions](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@8.2/manual/InstallURPIntoAProject.html) if you are trying to install the URP into an existing project.
  \item Additionally, these packages must be installed from the package manager:
    \begin{itemize}
      \item \textbf{Input System}
      \item \textbf{XR Interaction Toolkit}
      \item \textbf{XR Plugin Management}
      \item The \textbf{XR plugin} for your respective device, e.g. the \textbf{Oculus XR Plugin}
    \end{itemize}
\end{itemize}

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
- Make sure that your XR plugin - e.g. Oculus - is properly set up (`Project Settings -> XR Plugin-in Management`). You can learn more [here](https://docs.unity3d.com/Manual/configuring-project-for-xr.html).

### Tags

To edit your project's tags and layers, select  ```Edit Layers``` from the ```Layers``` dropdown (see Fig. \ref{fig:EditTagsAndLayers}). You can also edit the tags in your project settings.

\begin{figure}[!h]
    \centering
    \includegraphics[width=.5\textwidth]{content/res/EditTagsAndLayers.png}
    \caption{To edit your project's tags and layers, select  `Edit Layers` from the `Layers` dropdown.}
    \label{fig:EditTagsAndLayers}
\end{figure}

Make sure the following tags exist (exact spelling matters):  

- ```Box Mask```
- ```Cylinder Mask```
- ```HandL```
- ```HandR```
- ```IndexL```
- ```IndexR```
- ```Level```
- ```PreviewCamera```
- ```PreviewScreen```
- ```Spotlight Mask```
- ```ThumbL```
- ```ThumbR```
- ```WIM Level Old```
- ```WIM```



### Layers

To edit your project's tags and layers, select  ```Edit Layers``` from the ```Layers``` dropdown (see Fig. \ref{fig:EditTagsAndLayers}). You can also edit the tags in your project settings.

Make sure the following layers exist (exact spelling matters):

- ```Fingers```
- ```Hands```
- ```PinchGrabbable```
- ```Player```
- ```WIM```


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
2. Add XR rig to the scene
   - Drag the 'XR Rig' prefab (`WIMVR/Prefabs/XR Rig`) into the scene
3. Add miniature model
   - Add *Miniature Model* prefab to scene
   - Make sure both the tag and layer are set to 'WIM'
   - Configure miniature model to your preferences (see [Chapter 1.6 'Configure Miniature Model'](#configure-miniature-model))
   - Press 'Generate WIM' button
4. Add a *PlayerInput* component to the WIM gameobject (see Fig. \ref{fig:PlayerInput})
   - Assign an input actions configuration file: You can either use the provided 'InputMapping' configuration or create a new one (`Assets -> Create -> Input Actions`).
   - [Optional] set the default scheme to XR
   - [Optional] set the default map to 'Miniature Model'
   - See Section \ref{section:ConfigureInput} on how to change the input mapping

## Configure Miniature Model \label{section:ConfigureMiniatureModel}

To configure the WIM, select the WIM gameobject in the scene. All settings are displayed in the *Miniature Model* component inspector.
The configuration is stored in a *WIMConfiguration* asset, so you have to assign one.
Therefore, you can either select an existing *WIMConfiguration* (see Fig. \ref{fig:AssignWIMConfiguration}) or crate a new one using the create menu (see Fig. \ref{fig:CreateWIMConfiguration}). You can quickly switch between multiple WIM configurations by exchanging the assigned *InputWIMConfigurationActions*.

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

\pagebreak

## Configure Input \label{section:ConfigureInput}

\begin{figure}[!h]
    \centering
    \includegraphics[width=\textwidth]{content/res/PlayerInput.png}
    \caption{Player input}
    \label{fig:PlayerInput}
\end{figure}

To configure the player input, select the WIM gameobject in the scene.
The configuration is stored in a *InputActions* asset, so you have to assign one.
Therefore, you can either select an existing *InputActions* (see Fig. \ref{fig:PlayerInput}) or crate a new one using the create menu (`Assets -> Create -> Input Actions`). You can quickly switch between multiple input mapping configurations by exchanging the assigned *InputActions* configuration.

To edit the input mappings, open the *InputActions* asset by double-clicking it. Then, you can select the action you wish to modify and assign a new binding, or map the existing binding to another button. For information on how to edit Input Action Assets in the dedicated editor, see [Action Assets](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/ActionAssets.html#editing-input-action-assets).

\pagebreak

\begin{figure}[!h]
    \centering
    \includegraphics[width=\textwidth]{content/res/VRController/VROculusController.png}
    \caption{Default input mappings. Image courtesy of Marie Louise Eichner.}
    \label{fig:DefaultInput}
\end{figure}

Default input mappings:

1. Grab/carry WIM
2. Respawn WIM
3.
4.
5. Vertical scrolling
6. Horizontal scrolling / rotate destination
7. Destination selection touch: start travel
8. Destination selection touch: select destination / detect arm length
9.
10. Grab/carry WIM

