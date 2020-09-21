// Author: Samuel Truman (contact@samueltruman.com)

using NUnit.Framework;
using WIMVR.Util;

namespace WIMVR.Tests.Features {
    public class TypeUtilsTests {
        public class GetOppositeHandTests {
            [Test]
            public void Given_None_Yield_None() {
                Assert.That(TypeUtils.GetOppositeHand(Hand.None), Is.EqualTo(Hand.None));
            }
            
            [Test]
            public void Given_Left_Yield_Right() {
                Assert.That(TypeUtils.GetOppositeHand(Hand.LeftHand), Is.EqualTo(Hand.RightHand));
            }
            
            [Test]
            public void Given_Right_Yield_Left() {
                Assert.That(TypeUtils.GetOppositeHand(Hand.RightHand), Is.EqualTo(Hand.LeftHand));
            }
        }
    }
}