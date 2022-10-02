using FakeItEasy;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WikiUpload;

namespace Tests.MiscellaneousTests
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
                .Returns(new Version("1.1.1.0"));
            _updateCheck = new UpdateCheck(_helpers, _gitbubPrevider);
        }

        [Test]
        public async Task When_NoVersionsAvailable_Then_NotNewerVerrsion()
        {
            var githubResponse = JsonConvert.SerializeObject(new List<GithubRelease>());
            A.CallTo(() => _gitbubPrevider.FetchLatestReleases(A<string>._))
                .Returns(githubResponse);

            var result = await _updateCheck.CheckForUpdates("", 0);

            Assert.That(result.IsNewerVersion, Is.False);
        }

        [Test]
        public async Task When_VersionIsNewer_Then_NewerVerrsion()
        {
            var versions = new List<string>
            {
                "v1.1.2",
                "v1.2.0",
                "v2.0.0",
            };

            foreach (var version in versions)
            {
                var githubResponse = JsonConvert.SerializeObject(new List<GithubRelease> {
                    new GithubRelease
                    {
                        TagName = version, HtmlUrl = "url", IsPrerelease = false,
                    }
                });
                A.CallTo(() => _gitbubPrevider.FetchLatestReleases(A<string>._))
                    .Returns(githubResponse);

                var result = await _updateCheck.CheckForUpdates("", 0);

                Assert.That(result.IsNewerVersion, Is.True, $"{version} should be a newer varsion");
            }
        }

        [Test]
        public async Task When_VersionIsOlderOrTheSame_Then_NotewerVerrsion()
        {
            var versions = new List<string>
            {
                "v1.1.1", // current version
                "v1.1.0",
                "v1.0.1",
                "v0.1.1",
            };

            foreach (var version in versions)
            {
                var githubResponse = JsonConvert.SerializeObject(new List<GithubRelease> {
                    new GithubRelease
                    {
                        TagName = version, HtmlUrl = "url", IsPrerelease = false,
                    }
                });
                A.CallTo(() => _gitbubPrevider.FetchLatestReleases(A<string>._))
                    .Returns(githubResponse);

                var result = await _updateCheck.CheckForUpdates("", 0);

                Assert.That(result.IsNewerVersion, Is.False, $"{version} should not ne a newer varsion");
            }
        }

        [Test]
        public async Task NonReleaseTagsAreIgnored()
        {
            var response = JsonConvert.SerializeObject(new List<GithubRelease>
            {
                new GithubRelease { TagName = "beta1.0.1", HtmlUrl = "", IsPrerelease=false },
                new GithubRelease { TagName = "v2.0.0", HtmlUrl = "", IsPrerelease=false },
            });
            A.CallTo(() => _gitbubPrevider.FetchLatestReleases(A<string>._))
                .Returns(response);

            var result = await _updateCheck.CheckForUpdates("", 0);

            Assert.That(result.LatestVersion, Is.EqualTo("2.0.0"));
        }

        [Test]
        public async Task PrereleasesAreIgnored()
        {
            var response = JsonConvert.SerializeObject(new List<GithubRelease>
            {
                new GithubRelease { TagName = "v3.0.0", HtmlUrl = "", IsPrerelease = true },
                new GithubRelease { TagName = "v2.0.0", HtmlUrl = "", IsPrerelease = false },
            });
            A.CallTo(() => _gitbubPrevider.FetchLatestReleases(A<string>._))
                .Returns(response);

            var result = await _updateCheck.CheckForUpdates("", 0);

            Assert.That(result.LatestVersion, Is.EqualTo("2.0.0"));
        }

        [Test]
        public async Task When_NewerVersion_Then_DetailsAreReturned()
        {
            var response = JsonConvert.SerializeObject(new List<GithubRelease>
            {
                new GithubRelease { TagName = "v2.0.0", HtmlUrl = "alpha", IsPrerelease = false },
            });
            A.CallTo(() => _gitbubPrevider.FetchLatestReleases(A<string>._))
                .Returns(response);

            var result = await _updateCheck.CheckForUpdates("", 0);

            Assert.That(result.Url, Is.EqualTo("alpha"));
            Assert.That(result.LatestVersion, Is.EqualTo("2.0.0"));
        }

        [Test]
        public async Task When_DelayIsSupplied_Then_CheckIsDelayed()
        {
            var response = JsonConvert.SerializeObject(new List<GithubRelease>
            {
                new GithubRelease { TagName = "v2.0.0", HtmlUrl = "alpha", IsPrerelease = false },
            });
            var delay = 666;

            _ = await _updateCheck.CheckForUpdates("", delay);

            A.CallTo(() => _helpers.Wait(delay))
                .MustHaveHappened(1, Times.Exactly);
        }

    }
}
