
\pagebreak

## Scaling (Pro)

**Description**  
Allows the player to resize the miniature model at runtime.

**How to Use in Game**  
To resize the miniature model, the player can grab it using both hands.
The WIM can be scaled up by moving the hands apart from each
other and scaled down by bringing the hands closer together.

**Setup**  
Add the *Scaling* scrip to the miniature model gameobject. Check 'Allow WIM Scaling' to enable.

**Configuration**  
The configuration is stored in a *ScalingConfig* asset. Select an existing *ScalingConfig* file or create a new one using the create menu. Therefore, right-click anywhere on the project window and select `Create -> WIM -> Feature Configureation -> Scaling`. Settings to configure:

- When scaling the miniature model up or down, the player changes the scale factor. The scale factor represents the miniature model's current size.
- Change *Min Scale Factor* to set the smallest allowed scale factor. This should be a positive number.
- Change *Max Scale Factor* to set the largest allowed scale factor. A scale factor of '1' would result in the miniature model being a 1:1 copy of the full-sized level, thus also full sized.
- *Scale Step* is used to determine how fast the scale factor can change. Small numbers will only allow small changes, large number will allow much faster scaling.
- *Inter Hand Distance Delta Threshold*: Ignore inter hand distance deltas below this threshold for scaling. Important so that not every tiny hand jitter will trigger scaling.

**Input Mappings**  
 -
