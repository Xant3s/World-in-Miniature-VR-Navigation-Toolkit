
\pagebreak

## Occlusion Handling: Melting Walls

\begin{figure}[!h]
    \centering
    \includegraphics[width=.5\textwidth]{content/res/MeltingWalls.png}
    \caption{Occlusion Handling Strategy: Melting Walls}
    \label{fig:MeltingWalls}
\end{figure}

**Description**  
As soon as the player's hand approaches the WIM, those parts of the WIM in close proximity to the hand start to fade. When the hand is moved elsewhere, the entire WIM is visible again.

**How to Use in Game**  
The player doesn't need to do anything to activate this feature. It will be automatically used when the player's hand is close to the miniature model.

**Setup**  
Add the *OcclusionHandling* script to the miniature model gameobject. Select 'Melting Walls' as 'Occlusion Handling Strategy' in the miniature model inspector.

**Configuration**  
The configuration is stored in an *OcclusionHandlingConfig* asset. Select an existing *OcclusionHandlingConfig* file or create a new one using the create menu. Therefore, right-click anywhere on the project window and select `Create -> WIM -> Feature Configureation -> Occlusion Handling`. Settings to configure:

- *Melt Radius*: Radius around the player's arm in which the miniature model is hidden.
- *Melt Height*: Distance parallel to the player's arm in which the miniature model is hidden.

**Input Mappings**  
 -
