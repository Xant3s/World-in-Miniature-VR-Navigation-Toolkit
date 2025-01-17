
\pagebreak

## Post-Travel Path Trace
<!-- TODO: Better image -->

\begin{figure}[!h]
    \centering
    \includegraphics[width=.5\textwidth]{content/res/PathTrace.png}
    \caption{Orientation Aid: Path Trace}
    \label{fig:PathTrace}
\end{figure}

**Description**  
The miniature model displays a line from the previous position to the new one, visualizing the recent locomotion. The path trace gradually fades over time, starting from the previous position and moving toward the new one.

**How to Use in Game**  
Will be displayed automatically.

**Setup**  
Add the *PathTrace* script to the miniature model gameobject. Check 'Post Travel Path Trace' to enable.

**Configuration**  
The configuration is stored in a *PathTraceConfig* asset. Select an existing *PathTraceConfig* file or create a new one using the create menu. To do that, right-click anywhere on the project window and select `Create -> WIM -> Feature Configuration -> Path Trace`. Settings to configure:

* 'Post Travel Path Trace': Whether this feature is enabled.
* 'Trace Duration': How long the path trace is visible in seconds.

**Input Mappings**  
 -