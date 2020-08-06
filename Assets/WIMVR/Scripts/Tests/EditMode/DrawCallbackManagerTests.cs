// Author: Samuel Truman (contact@samueltruman.com)


using NUnit.Framework;
using UnityEngine.UIElements;
using WIMVR.Core;
using WIMVR.Editor.Core;
using Assert = UnityEngine.Assertions.Assert;

namespace WIMVR.Tests {
    public class DrawCallbackManagerTests {
        public class GetNumberOfCallbacks {
            [Test]
            public void Given_Empty_Then_Return_Zero_Callbacks() {
                DrawCallbackManager manager = A.DrawCallbackManager;
                Assert.AreEqual(0, manager.GetNumberOfCallbacks());
            }
        }
        
        
        public class AddCallbackTests {
            void DummyCallback(WIMConfiguration WIMConfig, VisualElement container){}

            // [Test]
            // public void Given_Empty_Then_Add_Callback_Return_Manager_With_Callback() {
            //     DrawCallbackManager manager = A.DrawCallbackManager;
            //     Assert.AreEqual(0, manager.GetNumberOfCallbacks());
            //     manager.AddCallback(DummyCallback);
            //     Assert.AreEqual(1, manager.GetNumberOfCallbacks());
            // }
        }
    }
}