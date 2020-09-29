// Author: Samuel Truman (contact@samueltruman.com)

using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace WIMVR.Tests {
    [TestFixture]
    public class GameObjectUtilsTests {
        public class GetOrAddComponentTests {
            [Test]
            public void Given_Camera_Then_Return_Camera() {
                GameObject obj = A.GameObject.WithComponent(typeof(Camera));
                obj.GetOrAddComponent<Camera>().Should().Be(obj.GetComponent<Camera>());
            }

            [Test]
            public void Given_Nothing_Then_Return_New_Camera() {
                GameObject obj = new GameObject();
                obj.GetOrAddComponent<Camera>().Should().NotBeNull();
                obj.GetComponent<Camera>().Should().NotBeNull();
            }
        }
    }
}