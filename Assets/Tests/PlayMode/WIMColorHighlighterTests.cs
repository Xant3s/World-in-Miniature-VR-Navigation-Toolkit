// Author: Samuel Truman (contact@samueltruman.com)

using System.Collections;
using FluentAssertions;
using UnityEngine;
using UnityEngine.TestTools;
using WIMVR.Features.Distance_Grab;
using WIMVR.Util;

namespace WIMVR.Tests.PlayMode {
    public class WIMColorHighlighterTests {
        private static readonly int tint = Shader.PropertyToID("_Tint");

        [UnityTest]
        public IEnumerator Test_Set_Highlight() {
            // Crate primitive.
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Renderer renderer = obj.GetComponent<Renderer>();

            // Assign WIM material.
            string shaderName = Resources.Load<Material>("WIM Material").shader.name;
            Material material = new Material(Shader.Find(shaderName));
            renderer.sharedMaterial = material;
            Color defaultColor = material.GetColor(tint);
            
            // Test enable highlight.
            IHighlighter highlighter = obj.AddComponent<WIMColorHighlighter>();
            highlighter.HighlightEnabled = true;
            yield return null;
            renderer.sharedMaterial.GetColor(tint).Should().NotBe(defaultColor);

            // Test disable highlight.
            highlighter.HighlightEnabled = false;
            yield return null;
            renderer.sharedMaterial.GetColor(tint).Should().Be(defaultColor);
        }

        [UnityTest]
        public IEnumerator Test_Set_Highlight_Together_With_Distance_Grabbable() {
            // Crate primitive with distance grabbable.
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Renderer renderer = obj.GetComponent<Renderer>();
            DistanceGrabbable grabbable = obj.AddComponent<DistanceGrabbable>();

            // Assign WIM material.
            string shaderName = Resources.Load<Material>("WIM Material").shader.name;
            Material material = new Material(Shader.Find(shaderName));
            renderer.sharedMaterial = material;
            Color defaultColor = material.GetColor(tint);

            // Test enable highlight.
            IHighlighter highlighter = obj.AddComponent<WIMColorHighlighter>();
            highlighter.HighlightEnabled = true;
            yield return null;
            renderer.sharedMaterial.GetColor(tint).Should().NotBe(defaultColor);

            // Test disable highlight.
            highlighter.HighlightEnabled = false;
            yield return null;
            renderer.sharedMaterial.GetColor(tint).Should().Be(defaultColor);
            // yield return null;
        }
    }
}