// Author: Samuel Truman (contact@samueltruman.com)


using System.Collections.Generic;
using WIMVR.Editor.Core;

namespace WIMVR.Tests {
    public class DrawCallbackManagerBuilder {
        private IEnumerable<Callback> callbacks = new List<Callback>();
        
        
        public struct Callback {
            public string Key;
            public DrawCallbackManager.InspectorAction Action;
            public int Priority;
        }

        public DrawCallbackManagerBuilder WithCallbacks(params Callback[] callbacks) {
            this.callbacks = callbacks;
            return this;
        }

        public DrawCallbackManager Build() {
            var manager = new DrawCallbackManager();
            foreach (var callback in callbacks) {
                manager.AddCallback(callback.Action, callback.Priority, callback.Key);
            }
            return manager;
        }

        public static implicit operator DrawCallbackManager(DrawCallbackManagerBuilder builder) {
            return builder.Build();
        }
    }
}