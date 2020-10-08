
\pagebreak

## Post-Travel Path Trace (Pro)
<!-- TODO: Better image -->

<!-- ![Orientation Aid: Path Trace](content/res/PathTrace.png) -->

\begin{figure}[!h]
    \centering
    \includegraphics[width=.5\textwidth]{content/res/PathTrace.png}
    \caption{OcclusOrientation Aid: Path Trace}
    \label{fig:PathTrace}
\end{figure}

**Description**  
A line from the previous position to the new position is displayed in the miniature model, visualizing the locomotion that just took place. The path trace is faded-out over time, from the previous position towards the new one.

**How to Use in Game**  
Will be displayed automatically.

**Setup**  
Add the *PathTrace* script to the miniature model gameobject. Check 'Post Travel Path Trace' to enable.

**Configuration**  
The configuration is stored in a *PathTraceConfig* asset. Select an existing *PathTraceConfig* file or create a new one using the create menu. Therefore, right-click anywhere on the project window and select `Create -> WIM -> Feature Configureation -> Path Trace`. Settings to configure:

* 'Post Travel Path Trace': Whether this feature is enabled.
* 'Trace Duration': How long the path trace is visible in seconds.

**Input Mappings**  
 -