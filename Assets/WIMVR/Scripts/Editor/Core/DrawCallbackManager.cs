// Author: Samuel Truman (contact@samueltruman.com)

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using WIMVR.Core;

namespace WIMVR.Editor.Core {
    /// <summary>
    /// Used by MiniatureModelEditor. Features can add additional UI utilizing callbacks.
    /// </summary>
    public class DrawCallbackManager : IDisposable {
        public delegate void InspectorAction(WIMConfiguration config, VisualElement container);

        private static IDictionary<string, IDictionary<int, InspectorAction>> OnDraw = new Dictionary<string, IDictionary<int, InspectorAction>>();

        
        public void AddCallback(InspectorAction callback, int priority = 0, string key = "") {
            if(OnDraw.ContainsKey(key)) {
                OnDraw[key][priority] = callback;
            }
            else {
                OnDraw.Add(key, new Dictionary<int, InspectorAction>());
                OnDraw[key][priority] = callback;
            }
        }

        public void RemoveCallback(InspectorAction callback, string key = "") {
            if(!OnDraw.ContainsKey(key)) return;
            foreach(var item in OnDraw[key].Where(pair => pair.Value == callback).ToList()) {
                OnDraw[key].Remove(item.Key);
            }
        }

        public int GetNumberOfCallbacks(string key = "") {
            return OnDraw.ContainsKey(key) ? OnDraw[key].Count : 0;
        }

        public void InvokeCallbacks(MiniatureModel WIM, VisualElement container, string key = "") {
            if(!OnDraw.ContainsKey(key)) return;
            var pairs = OnDraw[key].ToList();
            pairs.Sort((x,y) =>x.Key.CompareTo(y.Key));
            pairs.ForEach(callback => callback.Value(WIM.Configuration, container));
        }

        private void ReleaseUnmanagedResources() {
            OnDraw.Clear();
        }

        public void Dispose() {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~DrawCallbackManager() {
            ReleaseUnmanagedResources();
        }
    }
}