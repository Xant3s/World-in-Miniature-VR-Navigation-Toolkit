// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections.Generic;
using System.Linq;
using WIMVR.Editor.Core;

namespace WIMVR.Tests {
    public class DrawCallbackManagerBuilder {
        private Callback[] callbacks;
        // private List<Callback> callbacks = new List<Callback>();


        public DrawCallbackManagerBuilder WithCallbacks(params Callback[] callbacks) {
            this.callbacks = callbacks;
            // this.callbacks = callbacks.ToList();
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