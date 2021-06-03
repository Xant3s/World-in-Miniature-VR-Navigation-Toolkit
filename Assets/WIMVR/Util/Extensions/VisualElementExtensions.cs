// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine.UIElements;

namespace WIMVR.Util.Extensions {
    public static class VisualElementExtensions {
        public static void Show(this VisualElement element) => element.SetVisible(true);

        public static void Hide(this VisualElement element) => element.SetVisible(false);

        public static void SetVisible(this VisualElement element, bool value) {
            var visible = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            var hidden = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            element.style.display = value ? visible : hidden;
        }
    }
}