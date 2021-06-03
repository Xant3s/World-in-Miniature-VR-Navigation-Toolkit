// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine.UIElements;

namespace WIMVR.Util.Extensions {
    public static class VisualElementExtensions {
        public static void Show(this VisualElement element) => element.SetVisible(true);

        public static void Hide(this VisualElement element) => element.SetVisible(false);

        public static void SetVisible(this VisualElement element, bool value) {
            var hidden = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            var visible = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            element.style.display = value ? hidden : visible;
        }
    }
}