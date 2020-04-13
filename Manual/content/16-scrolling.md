
\pagebreak

## Scrolling  (Pro)

<!-- TODO: image -->
**Description**  
When enabled, only an excerpt of the miniature model will be visible. Allows the player to decide which part of the miniature model should be visible at runtime. Especially useful when the level is very large and thus the miniature model would be very large also.

**How to Use in Game**  
There are two possible scrolling modes. The automatic  scrolling mode is useful in combination with the destination selection method 'touch', which is described in Section 2.2. If a destination is selected, the visual excerpt will
be centered on the destination. The visual excerpt will be centered on the user representation otherwise. Using the automatic scrolling mode requires no additional user interaction.  

The manual scrolling mode is useful in combination with the 'pickup' destination selection method described in Section
2.1. Using the manual scrolling mode, the user can use a thumbstick to move the visual excerpt of the miniature model.

**Setup**  
Add the *Scrolling* script to the miniature model gameobject. Check 'Allow WIM Scrolling' to enable.

**Configuration**  
The configuration is stored in a *ScrollingConfig* asset. Select an existing *ScrollingConfig* file or create a new one using the create menu. Therefore, right-click anywhere on the project window and select `Create -> WIM -> Feature Configureation -> Scrolling`. Settings to configure:

* 'Allow WIM Scrolling': whether this feature is enabled
* 'Active Area Bounds': Bounds of the box collider the player can use to grab the miniature model.
* 'Auto Scroll': Whether scrolling should be automated, so that the destination indicator is centered. If no destination is selected, the player is centered. If disabled, the player can scroll manually using thumbsticks.
* 'Scroll Speed': The default scroll speed. Only available when 'Auto Scroll' is enabled.
* 'Allow Vertical Scrolling': Whether vertical scrolling should be allowed. This required a second thumbstick. Only available when 'Auto Scroll' is disabled.

**Input**  
When 'Auto Scroll' is enabled, no input is required. Otherwise, configure 'Scrolling Axis' in the input manager. Also configure 'Vertical Scrolling Axis' in the input manager, if 'Allow Vertical Scrolling' is enabled.
