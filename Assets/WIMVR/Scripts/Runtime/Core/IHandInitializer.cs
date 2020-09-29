// Author: Samuel Truman (contact@samueltruman.com)

using System;

namespace WIMVR.Core {
    public interface IHandInitializer<out T> {
        event Action<T> OnRightHandInitialized;
        event Action<T> OnLeftHandInitialized;
        void StartWaitForHands();
    }
}