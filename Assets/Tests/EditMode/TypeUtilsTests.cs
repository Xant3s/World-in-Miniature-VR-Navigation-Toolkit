// Author: Samuel Truman (contact@samueltruman.com)

using FluentAssertions;
using NUnit.Framework;
using WIMVR.Util;

namespace WIMVR.Tests.Features {
    [TestFixture]
    public class TypeUtilsTests {
        public class GetOppositeHandTests {
            [Test]
            public void Given_None_Yield_None() {
                TypeUtils.GetOppositeHand(Hand.None).Should().Be(Hand.None);
            }
            
            [Test]
            public void Given_Left_Yield_Right() {
                TypeUtils.GetOppositeHand(Hand.LeftHand).Should().Be(Hand.RightHand);
            }
            
            [Test]
            public void Given_Right_Yield_Left() {
                TypeUtils.GetOppositeHand(Hand.RightHand).Should().Be(Hand.LeftHand);
            }
        }
    }
}