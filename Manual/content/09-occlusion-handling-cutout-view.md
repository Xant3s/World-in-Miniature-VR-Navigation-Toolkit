
\pagebreak

## Occlusion Handling: Cutout View

<!-- TODO: image -->
**Description**  
Geometries which are within proximity of an area
defined by a cylinder are clipped. As opposed to the “melting walls” technique, the cylinder is attached to the user's head.

**How to Use in Game**  
The user doesn't need to do anything to activate this feature. It will be automatically used when the user's head is close to the miniature model.

**Setup**  
Add the *OcclusionHandling* script to the miniature model gameobject. Select 'Cutout View' as 'Occlusion Handling Strategy' in the miniature model inspector.

**Configuration**  
The configuration is stored in an *OcclusionHandlingConfig* asset. Select an existing *OcclusionHandlingConfig* file or create a new one using the create menu. To do that, right-click anywhere on the project window and select `Create -> WIM -> Feature Configuration -> Occlusion Handling`. Settings to configure:

- *Cutout Range*: Height of the view cone hiding the miniature model. The pointy end origins at the user's eyes.
- *Cutout Angle*: Angle of the view cone hiding the miniature model.
- *Show Cutout Light*: Whether the view cone should be visualized by a spotlight.
- *Cutout Light Color*: The color of the spotlight. Only available if 'Show Cutout Light' is enabled.

**Input Mappings**  
 -
