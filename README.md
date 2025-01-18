# WIMVR: World-in-Miniature VR Navigation Toolkit

## What is World-in-Miniature (WIM)?

Navigation is one of the most fundamental challenges
in Virtual Reality (VR). The world-in-miniature (WIM) metaphor allows players to travel in large-scale virtual environments (VEs) regardless of available physical space while maintaining a high-level overview of the VE. It relies on a hand-held, scaled-down duplicate of the entire VE, where the user’s current position is displayed, and an interface provided to introduce his/her next movements. This scaled-down version of the VE is called WIM or miniature model.

## What is WIMVR?

World-in-Miniature VR Navigation Toolkit (WIMVR) is a Unity plugin that integrates and extends state-of-the-art interaction tasks and
visualization concepts to overcome open conceptual gaps and to
provide a comprehensive practical solution for traveling in VR.

## Features

[![Watch the video](https://img.youtube.com/vi/DzmdxMqrKJw/maxresdefault.jpg)](https://www.youtube.com/watch?v=DzmdxMqrKJw)

- Automatic WIM generation
- Destination selection:
  - via pickup gesture
  - via touch
- Grab/carry the WIM
- WIM respawn
- Distance grab
- Scalability:
  - Scaling
  - Scrolling
- Occlusion handling:
  - transparency
  - melting walls
  - cutout view
- User guidance:
  - Preview screen
  - travel preview animation
  - path trace
  - WIM respawn dissolve FX
- Experimental: detect user's arm length
- Experimental: live update the WIM

## How to Test


### Hardware Requirements

- tested with Oculus Quest and Unity 2020.1.10f1
- Recommended: Oculus Link cable

### Requirements

1. Install [correct Unity version](ProjectSettings/ProjectVersion.txt) (recommended: install via [Unity Hub](https://unity3d.com/de/get-unity/download))
    - Make sure to also install modules:
      - Android Build Support
      - Android SDK & NDK Tools
      - OpenJDK
    - Or add modules to existing Unity installation (Unity Hub > Installs > Add Modules)
2. Install [Oculus App](https://www.oculus.com/setup/)

### Installation

1. Clone this repository  
   ```$ git clone --recursive --depth 1 git@github.com:WIMVR-Plugin/WIMVR-Deployment.git```
2. Open in Unity
3. Make sure the build target is set to Android (File > Build Settings)
4. Make sure `OpenGLES3` is the only active graphics API (Edit > Project Settings > Player > Other Settings)
5. Make sure the Oculus Plugin is enabled for both PC and Android (Edit > Project Settings > XR Plug-in Management)

### How to Run Example

The `SimpleExample` scene can be used to test all features. There are two different methods to run this example.

1. Test via Oculus Link
   1. Make sure the Oculus App is running
   2. Connect the Oculus Quest to your PC using the Oculus Link cable
   3. Start Oculus Link (select Settings > Oculus Link from the Quest main menu)
   4. Press play in Unity
2. Test via standalone
   1. Connect the Oculus Quest to your PC
   2. Make sure Oculus Links is *not* running
   3. Build and run (File > Build & Run)
   4. No cable connection is required once the build process is finished and the .apk was transferred to the Quest

### Common Oculus Quest Issues

- **No image is rendered for the right eye** - set the Stereo Rendering Mode to Multiview (Edit > Project Settings > XR Plug-in Management > Oculus > Android)
- **Massive performance issues when running in standalone mode** - uncheck the 'Auto-Switch' setting on the PlayerInput component attached to the WIM instance


## Scientific Reference

If you need a scientific reference, you can cite this [paper](https://dl.acm.org/doi/10.1145/3402942.3402994), which is available for free [here](https://www.researchgate.net/publication/344368828_An_Integrated_Design_of_World-in-Miniature_Navigation_in_Virtual_Reality). It also provides additional insights into World-in-Miniature navigation.

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