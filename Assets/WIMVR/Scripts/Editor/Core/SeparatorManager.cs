// Author: Samuel Truman (contact@samueltruman.com)

using UnityEngine.UIElements;

namespace WIMVR.Editor.Core {
    public class SeparatorManager : ISeparatorManager {
        private readonly VisualElement root;


        public SeparatorManager(VisualElement root) {
            this.root = root;
        }
        
        public void RegisterUnique(string text = "", ushort space = 20) {
            var separatorName = GetSeparatorName(text);
            if(root.Q<Label>(separatorName) != null) return;
            var newSeparator = new Label(text){name = separatorName};
            root.Add(newSeparator);
            newSeparator.AddToClassList("Separator");
        }

        public void UnregisterUnique(string text = "") {
            var separatorName = GetSeparatorName(text);
            var separator = root.Q<Label>(separatorName);
            if(separator == null) return;
            root.Remove(separator);
        }

        private static string GetSeparatorName(string text) => $"separator-{text}";
    }
}