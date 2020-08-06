// Author: Samuel Truman (contact@samueltruman.com)

using NUnit.Framework;
using UnityEngine.UIElements;
using WIMVR.Core;
using WIMVR.Editor.Core;
using Assert = UnityEngine.Assertions.Assert;

namespace WIMVR.Tests {
    public class DrawCallbackManagerTests {
        public class GetNumberOfCallbacks {
            void DummyCallback(WIMConfiguration WIMConfig, VisualElement container) { }


            [Test]
            public void Given_Empty_Then_Return_Zero_Callbacks() {
                DrawCallbackManager manager = A.DrawCallbackManager;
                Assert.AreEqual(0, manager.GetNumberOfCallbacks());
            }
            
            [Test]
            public void Return_Correct_Number_Of_Callbacks_One() {
                Callback callback = A.Callback.WithAction(DummyCallback).WithPriority(1);
                DrawCallbackManager manager = A.DrawCallbackManager.WithCallbacks(callback);
                Assert.AreEqual(1, manager.GetNumberOfCallbacks());
                manager.Dispose();
            }

            [Test]
            public void Return_Correct_Number_Of_Callbacks_Two() {
                Callback callback = A.Callback.WithAction(DummyCallback);
                Callback callback2 = A.Callback.WithAction(DummyCallback).WithPriority(1);
                DrawCallbackManager manager = A.DrawCallbackManager.WithCallbacks(callback, callback2);
                Assert.AreEqual(2, manager.GetNumberOfCallbacks());
                manager.Dispose();
            }
            
            [Test]
            public void Return_Correct_Number_Of_Callbacks_Multiple_Keys() {
                Callback callback = A.Callback.WithAction(DummyCallback);
                Callback callback2 = A.Callback.WithAction(DummyCallback).WithKey("test");
                Callback callback3 = A.Callback.WithAction(DummyCallback).WithKey("test").WithPriority(2);
                DrawCallbackManager manager = A.DrawCallbackManager.WithCallbacks(callback, callback2, callback3);
                Assert.AreEqual(1, manager.GetNumberOfCallbacks());
                Assert.AreEqual(2, manager.GetNumberOfCallbacks("test"));
                manager.Dispose();
            }
        }


        public class AddCallbackTests {
            void DummyCallback(WIMConfiguration WIMConfig, VisualElement container) { }

            [Test]
            public void Given_Empty_Then_Add_Callback_Return_Manager_With_Callback() {
                DrawCallbackManager manager = A.DrawCallbackManager;
                Assert.AreEqual(0, manager.GetNumberOfCallbacks());
                manager.AddCallback(DummyCallback);
                Assert.AreEqual(1, manager.GetNumberOfCallbacks());
                manager.Dispose();
            }
        }
    }
}