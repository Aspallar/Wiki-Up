using FakeItEasy;
using NUnit.Framework;
using WikiUpload;

namespace Tests
{
    [TestFixture]
    public class YoutubeTests
    {
        private IHelpers _helpers;
        private Youtube _youtube;

        [SetUp]
        public void Setup()
        {
            _helpers = A.Fake<IHelpers>();
            _youtube = new Youtube(_helpers);
        }

        [Test]
        public void WhenPlaylistIdIsPresent_Then_IdIsExtracted()
        {
            var playlistId = _youtube.ExtractPlaylistId("https://youtube.com?list=ABC&foo=foo");

            Assert.That(playlistId, Is.EqualTo("ABC"));
        }

        [Test]
        public void WhenPlaylistIdIsAbsent_Then_NullIsReturned()
        {
            var playlistId = _youtube.ExtractPlaylistId("https://youtube.com?foo=foo");

            Assert.That(playlistId, Is.Null);
        }

        [Test]
        public void WhenNotYoutubeUrl_Then_NullIsReturned()
        {
            var playlistId = _youtube.ExtractPlaylistId("https://footube.com?list=ABC&foo=foo");

            Assert.That(playlistId, Is.Null);
        }
    }
}
