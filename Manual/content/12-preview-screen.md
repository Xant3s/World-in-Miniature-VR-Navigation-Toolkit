
\pagebreak

## Preview Screen (Pro)

\begin{figure}[!h]
    \centering
    \includegraphics[width=.5\textwidth]{content/res/PreviewScreen.png}
    \caption{Orientation Aid: Preview Screen}
    \label{fig:PreviewScreen}
\end{figure}

**Description**  
The preview screen is a 2D panel that floats
in the VE and always faces the user. On the preview screen, the user can see a live preview of what the currently specified destination looks like, i.e. a continuously updated image showing exactly what the user will see once translated and rotated to the specified location and orientation. There are two different preview screen modes.

**How to Use in Game**  
When the automatic mode is active, the WIM will be automatically displayed while a destination is selected. The preview screen will float above the WIM using a consistent offset. No user input is required.

The pickup mode works very similarly to the pickup destination selection method described in Section \ref{section:DestinationSelectionPickup}. The destination indicator’s view frustum is displayed in the miniature model. The user can grab the view frustum by pinching the index finger and thumb. The preview screen will become visible and attached to the user’s hand. Afterwards, the user can position the preview screen anywhere in the VE. As soon as the preview screen is dropped, it will retain its position relative to the WIM. The preview screen can be closed by pressing a Close button which is displayed
in the upper right corner of the preview screen.

**Setup**  
Add *PreviewScreen* script to miniature model gameobject. Check 'Show Preview Screen' to enable.

**Configuration**  
The configuration is stored in an *PreviewScreenConfig* asset. Select an existing *PreviewScreenConfig* file or create a new one using the create menu. Therefore, right-click anywhere on the project window and select `Create -> WIM -> Feature Configureation -> Preview Screen`. Settings to configure:

- *Show Preview Screen*: Whether this feature is enabled.
- *Auto Position Preview Screen*: Whether the preview screen should be automatically opened and placed at a fixed position above the miniature model. Otherwise, the player has to grab the destination indicator's view cone using index and thumb and place the preview screen somewhere.

**Input Mappings**  
 -