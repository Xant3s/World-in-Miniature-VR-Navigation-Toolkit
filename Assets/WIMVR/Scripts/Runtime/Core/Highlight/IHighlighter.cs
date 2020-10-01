// Author: Samuel Truman (contact@samueltruman.com)

namespace WIMVR.Util {
    public interface IHighlighter {
        /// <summary>
        /// Whether the highlight effect is active.
        /// Changing the value will enable or disable the hightlight effect.
        /// </summary>
        bool HighlightEnabled { get; set; }
    }
}