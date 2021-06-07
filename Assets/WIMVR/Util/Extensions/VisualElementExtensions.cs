// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine.UIElements;

namespace WIMVR.Util.Extensions {
    public static class VisualElementExtensions {
        public static void Show(this VisualElement element) => element.SetDisplay(true);

        public static void Hide(this VisualElement element) => element.SetDisplay(false);

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