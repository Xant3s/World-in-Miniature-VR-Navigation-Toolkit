// Author: Samuel Truman (contact@samueltruman.com)

using System;
using UnityEngine;

namespace WIMVR.Tests {
    public class GameObjectBuilder {
        private Type[] components;

        public GameObjectBuilder WithComponent(params Type[] components) {
            this.components = components;
            return this;
        }

        public GameObject Build() {
            GameObject obj = new GameObject();
            foreach(var component in components) {
                obj.AddComponent(component);
            }
            return obj;
        }

        public static implicit operator GameObject(GameObjectBuilder builder) {
            return builder.Build();
        }
    }
}