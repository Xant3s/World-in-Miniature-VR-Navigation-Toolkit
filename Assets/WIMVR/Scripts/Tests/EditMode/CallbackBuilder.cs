// Author: Samuel Truman (contact@samueltruman.com)

using WIMVR.Editor.Core;

namespace WIMVR.Tests {
    public class Callback {
        public DrawCallbackManager.InspectorAction Action;
        public int Priority = 0;
        public string Key = "";

        public Callback(DrawCallbackManager.InspectorAction action, int priority = 0, string key = "") {
            Key = key;
            Action = action;
            Priority = priority;
        }
    }

    public class CallbackBuilder {
        private DrawCallbackManager.InspectorAction action;
        private int priority;
        private string key;

        public CallbackBuilder WithKey(string key) {
            this.key = key;
            return this;
        }

        public CallbackBuilder WithAction(DrawCallbackManager.InspectorAction action) {
            this.action = action;
            return this;
        }

        public CallbackBuilder WithPriority(int priority) {
            this.priority = priority;
            return this;
        }

        public Callback Build() {
            return new Callback(action, priority, key);
        }

        public static implicit operator Callback(CallbackBuilder builder) {
            return builder.Build();
        }
    }
}