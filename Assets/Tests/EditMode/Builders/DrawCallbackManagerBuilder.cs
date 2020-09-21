// Author: Samuel Truman (contact@samueltruman.com)

using WIMVR.Editor.Core;

namespace WIMVR.Tests {
    public class DrawCallbackManagerBuilder {
        private Callback[] callbacks;


        public DrawCallbackManagerBuilder WithCallbacks(params Callback[] callbacks) {
            this.callbacks = callbacks;
            return this;
        }

        public DrawCallbackManager Build() {
            var manager = new DrawCallbackManager();
            if (callbacks != null) {
                foreach (var callback in callbacks) {
                    if (callback.Key == null) callback.Key = "";
                    manager.AddCallback(callback.Action, callback.Priority, callback.Key);
                }
            }
            return manager;
        }

        public static implicit operator DrawCallbackManager(DrawCallbackManagerBuilder builder) {
            return builder.Build();
        }
    }
}