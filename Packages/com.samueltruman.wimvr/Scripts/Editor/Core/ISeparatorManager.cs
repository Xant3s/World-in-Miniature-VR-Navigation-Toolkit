// Author: Samuel Truman (contact@samueltruman.com)

namespace WIMVR.Editor.Core {
    public interface ISeparatorManager {
        void RegisterUnique(string text = "", ushort space = 20);
        void UnregisterUnique(string text = "");
    }
}