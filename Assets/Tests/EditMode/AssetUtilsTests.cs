using FluentAssertions;
using NUnit.Framework;
using WIMVR.Editor.Util;

namespace WIMVR.Tests.EditMode {
    [TestFixture]
    public class AssetUtilsTests {
        public class RemoveRedundanciesTests {
            [Test]
            public void Given_Path_Without_Redundancies_Returns_Path() {
                string pathWithoutRedundancies = "path/without/redundancies/file.ext";
                AssetUtils.RemoveRedundanciesFromPath(pathWithoutRedundancies).Should().Be(pathWithoutRedundancies);
            }

            [Test]
            public void Given_Path_With_Redundancy_Returns_Path_Without_Redundancy() {
                string pathWithRedundancy = "path/with/redundancy/../file.ext";
                string pathWithoutRedundancy = "path/with/file.ext";
                AssetUtils.RemoveRedundanciesFromPath(pathWithRedundancy).Should().Be(pathWithoutRedundancy);

            }

            [Test]
            public void Given_Path_With_Sequential_Redundancies_Returns_Path_Without_Redundancies() {
                string pathWithSequentialRedundancies = "path/with/redundancy/../../file.ext";
                string pathWithoutRedundancies = "path/file.ext";
                AssetUtils.RemoveRedundanciesFromPath(pathWithSequentialRedundancies).Should().Be(pathWithoutRedundancies);
            }
        }
    }
}