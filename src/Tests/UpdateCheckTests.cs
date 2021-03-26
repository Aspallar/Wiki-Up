using FakeItEasy;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using WikiUpload;

namespace Tests
{
    [TestFixture]
    public class UpdateCheckTests
    {
        private IGithubProvider _gitbubPrevider;
        private IHelpers _helpers;
        private UpdateCheck _updateCheck;

        [SetUp]
        public void Setup()
        {
            _gitbubPrevider = A.Fake<IGithubProvider>();
            _helpers = A.Fake<IHelpers>();
            A.CallTo(() => _helpers.ApplicationVersion)
                .Returns(new Version("1.0.0.0"));
            _updateCheck = new UpdateCheck(_helpers, _gitbubPrevider);
        }

        [Test]
        public async Task When_ResponseIsGarbage_Then_NotNewerVersion()
        {
            A.CallTo(() => _gitbubPrevider.FetchLatestRelease(A<string>._))
                .Returns("Garbage");

            var result = await _updateCheck.CheckForUpdates("", 0);

            Assert.That(result.IsNewerVersion, Is.False);
        }

        [Test]
        public async Task When_IsNewerVersion_Then_VersionReturned()
        {
            A.CallTo(() => _gitbubPrevider.FetchLatestRelease(A<string>._))
                .Returns(@"{
                    ""tag_name"": ""v1.0.1""
                 }");

            var result = await _updateCheck.CheckForUpdates("", 0);

            Assert.That(result.IsNewerVersion, Is.True);
            Assert.That(result.LatestVersion, Is.EqualTo("1.0.1"));
        }

        [Test]
        public async Task When_IsNewerVersion_Then_UrlReturned()
        {
            A.CallTo(() => _gitbubPrevider.FetchLatestRelease(A<string>._))
                .Returns(@"{
                    ""tag_name"": ""v1.0.1"",
                    ""html_url"": ""foobar""
                 }");

            var result = await _updateCheck.CheckForUpdates("", 0);

            Assert.That(result.IsNewerVersion, Is.True);
            Assert.That(result.Url, Is.EqualTo("foobar"));
        }

        [Test]
        public async Task When_IsSameVersion_Then_IsNewerVersionIsFalse()
        {
            A.CallTo(() => _gitbubPrevider.FetchLatestRelease(A<string>._))
                .Returns(@"{
                    ""tag_name"": ""v1.0.0"",
                    ""html_url"": ""foobar""
                 }");

            var result = await _updateCheck.CheckForUpdates("", 0);

            Assert.That(result.IsNewerVersion, Is.False);
        }

        [Test]
        public async Task When_InvalidTag_Then_IsNewerVersionIsFalse()
        {
            A.CallTo(() => _gitbubPrevider.FetchLatestRelease(A<string>._))
                .Returns(@"{
                    ""tag_name"": ""v1.foo.0"",
                    ""html_url"": ""foobar""
                 }");

            var result = await _updateCheck.CheckForUpdates("", 0);

            Assert.That(result.IsNewerVersion, Is.False);
        }

        [Test]
        public async Task When_DelayIsSupplied_Then_CheckIsDelayed()
        {
            A.CallTo(() => _gitbubPrevider.FetchLatestRelease(A<string>._))
                .Returns(@"{
                    ""tag_name"": ""v1.1.0"",
                 }");
            var delay = 666;

            _ = await _updateCheck.CheckForUpdates("", delay);

            A.CallTo(() => _helpers.Wait(delay))
                .MustHaveHappened(1, Times.Exactly);
        }

    }
}
