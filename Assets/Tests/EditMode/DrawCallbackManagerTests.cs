// Author: Samuel Truman (contact@samueltruman.com)

using FluentAssertions;
using NUnit.Framework;
using UnityEngine.UIElements;
using WIMVR.Core;
using WIMVR.Editor.Core;

namespace WIMVR.Tests {
    [TestFixture]
    public class DrawCallbackManagerTests {
        public class GetNumberOfCallbacks {
            void DummyCallback(WIMConfiguration WIMConfig, VisualElement container) { }


            [Test]
            public void Given_Empty_Then_Return_Zero_Callbacks() {
                DrawCallbackManager manager = A.DrawCallbackManager;
                manager.GetNumberOfCallbacks().Should().Be(0);
            }
            
            [Test]
            public void Return_Correct_Number_Of_Callbacks_One() {
                Callback callback = A.Callback.WithAction(DummyCallback).WithPriority(1);
                DrawCallbackManager manager = A.DrawCallbackManager.WithCallbacks(callback);
                
                manager.GetNumberOfCallbacks().Should().Be(1);
                
                manager.Dispose();
            }

            [Test]
            public void Return_Correct_Number_Of_Callbacks_Two() {
                Callback callback = A.Callback.WithAction(DummyCallback);
                Callback callback2 = A.Callback.WithAction(DummyCallback).WithPriority(1);
                DrawCallbackManager manager = A.DrawCallbackManager.WithCallbacks(callback, callback2);
                
                manager.GetNumberOfCallbacks().Should().Be(2);
                
                manager.Dispose();
            }
            
            [Test]
            public void Return_Correct_Number_Of_Callbacks_Multiple_Keys() {
                Callback callback = A.Callback.WithAction(DummyCallback);
                Callback callback2 = A.Callback.WithAction(DummyCallback).WithKey("test");
                Callback callback3 = A.Callback.WithAction(DummyCallback).WithKey("test").WithPriority(2);
                DrawCallbackManager manager = A.DrawCallbackManager.WithCallbacks(callback, callback2, callback3);

                manager.GetNumberOfCallbacks().Should().Be(1);
                manager.GetNumberOfCallbacks("test").Should().Be(2);
                
                manager.Dispose();
            }
        }

        public class AddCallbackTests {
            void DummyCallback(WIMConfiguration WIMConfig, VisualElement container) { }

            [Test]
            public void Given_Empty_Then_Add_Callback_Return_Manager_With_Callback() {
                DrawCallbackManager manager = A.DrawCallbackManager;
                
                manager.GetNumberOfCallbacks().Should().Be(0);
                
                manager.AddCallback(DummyCallback);
                
                manager.GetNumberOfCallbacks().Should().Be(1);
                
                manager.Dispose();
            }

            [Test]
            public void Given_Empty_Then_Add_Callback_With_Key() {
                Callback callback = A.Callback.WithAction(DummyCallback).WithKey("test");
                DrawCallbackManager manager = A.DrawCallbackManager.WithCallbacks(callback);
                
                manager.GetNumberOfCallbacks().Should().Be(0);
                manager.GetNumberOfCallbacks("test").Should().Be(1);
                
                manager.Dispose();
            }

            [Test]
            public void Given_Callback_Then_Add_Callback_With_Same_Priority_And_Key_Then_Override() {
                const int priority = 1;
                const string key = "test";
                Callback callback = A.Callback.WithAction(DummyCallback).WithKey(key).WithPriority(priority);
                Callback callback2 = A.Callback
                    .WithAction((WIMConfiguration WIMConfig, VisualElement container) => {
                        container.Add(new VisualElement());
                    })
                    .WithKey(key)
                    .WithPriority(priority);
                Callback callback3 = A.Callback.WithAction(DummyCallback).WithKey(key).WithPriority(priority);
                DrawCallbackManager manager = A.DrawCallbackManager.WithCallbacks(callback, callback2,callback3);
                
                manager.GetNumberOfCallbacks(key).Should().Be(1);
                
                manager.Dispose();
            }
        }
        
        public class RemoveCallbackTests {
            void DummyCallback(WIMConfiguration WIMConfig, VisualElement container) { }


            [Test]
            public void Given_One_Then_Remove_Yields_Empty() {
                Callback callback = A.Callback.WithAction(DummyCallback).WithPriority(1);
                DrawCallbackManager manager = A.DrawCallbackManager.WithCallbacks(callback);
                
                manager.RemoveCallback(DummyCallback);
                
                manager.GetNumberOfCallbacks().Should().Be(0);
                manager.Dispose();
            }

            [Test]
            public void Given_Two_With_Different_keys_Then_Remove_Yields_One() {
                const string key = "test";
                Callback callback = A.Callback.WithAction(DummyCallback);
                Callback callback2 = A.Callback.WithAction(DummyCallback).WithKey(key);
                DrawCallbackManager manager = A.DrawCallbackManager.WithCallbacks(callback, callback2);
                
                manager.RemoveCallback(DummyCallback, key);
                
                manager.GetNumberOfCallbacks().Should().Be(1);
                manager.GetNumberOfCallbacks(key).Should().Be(0);
                
                manager.Dispose();
            }

            [Test]
            public void Given_Two_Then_Remove_One_Yields_One() {
                Callback callback = A.Callback.WithAction(DummyCallback);
                Callback callback2 = A.Callback
                    .WithAction((WIMConfiguration WIMConfig, VisualElement container) => {
                        container.Add(new VisualElement());
                    })
                    .WithPriority(1);
                DrawCallbackManager manager = A.DrawCallbackManager.WithCallbacks(callback, callback2);
                
                manager.RemoveCallback(DummyCallback);
                
                manager.GetNumberOfCallbacks().Should().Be(1);

                manager.Dispose();
            }

            [Test]
            public void Given_Two_Then_Remove_Two_Yields_Zero() {
                Callback callback = A.Callback.WithAction(DummyCallback);
                Callback callback2 = A.Callback.WithAction(DummyCallback).WithPriority(1);
                DrawCallbackManager manager = A.DrawCallbackManager.WithCallbacks(callback, callback2);
                
                manager.RemoveCallback(DummyCallback);
                
                manager.GetNumberOfCallbacks().Should().Be(0);
                manager.Dispose();
            }

            [Test]
            public void Given_Empty_Then_Remove_Yields_No_Effect() {
                DrawCallbackManager manager = A.DrawCallbackManager;
                
                manager.RemoveCallback(DummyCallback);
                
                manager.GetNumberOfCallbacks().Should().Be(0);
                
                manager.Dispose();
            }
        }
    }
}