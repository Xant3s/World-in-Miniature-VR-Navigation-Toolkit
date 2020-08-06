// Author: Samuel Truman (contact@samueltruman.com)

using NUnit.Framework;
using UnityEngine;

namespace WIMVR.Tests {
    public class GameObjectUtilsTests {
        
        public class GetOrAddComponentTests {
            [Test]
            public void Given_Camera_Then_Return_Camera() {
                GameObject obj = A.GameObject.WithComponent(typeof(Camera));
                Assert.AreEqual(obj.GetComponent<Camera>(), 
                    obj.GetOrAddComponent<Camera>());
            }

            [Test]
            public void Given_Nothing_Then_Return_New_Camera() {
                GameObject obj = new GameObject();
                Assert.IsNotNull(obj.GetOrAddComponent<Camera>());
                Assert.IsNotNull(obj.GetComponent<Camera>());
            }
        }
    }
}