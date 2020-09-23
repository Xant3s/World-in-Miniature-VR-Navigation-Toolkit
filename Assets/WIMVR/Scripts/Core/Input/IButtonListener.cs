// Author: Samuel Truman (contact@samueltruman.com)

using System;

namespace WIMVR.Core.Input {
    internal interface IButtonListener {
        event Action OnButtonDown;
        event Action OnButtonUp;
        void Update();
    }
}