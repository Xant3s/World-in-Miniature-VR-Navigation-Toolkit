using UnityEngine.UIElements;

namespace WIMVR.Editor.Core {
    public static class VEExtension {
        /// <summary>
        /// Changes the VisualElement display state. If element is not displayed, it is invisible and doesn't use any space.
        /// </summary>
        /// <param name="element">The element to change.</param>
        /// <param name="value">Whether to display element.</param>
        public static void SetDisplay(this VisualElement element, bool value) {
            element.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}