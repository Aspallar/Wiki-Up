using NUnit.Framework;
using WikiUpload;

namespace Tests
{
    [TestFixture]
    public class UploadResponseTests
    {
        private readonly string _response01 = @"<?xml version=""1.0""?>
<api>
  <upload result=""Warning"" filekey=""186owl0hsxdc.ktry8i.33109274.png"" sessionkey=""186owl0hsxdc.ktry8i.33109274.png"">
    <warnings duplicate-archive=""Address-book-new.png"" exists=""Address-book-new.png"">
      <duplicate>
        <duplicate>Address-book-new.png</duplicate>
      </duplicate>
      <nochange timestamp=""2021-03-07T22:20:08Z"" />
    </warnings>
  </upload>
</api>";

        [SetUp]
        public void Setup()
        {
            UploadResponse.Initialize();
        }

        [Test]
        public void WarnningsTextPlaceFriendlyShortMessageFirst()
        {
            var response = new UploadResponse(_response01, "");

            Assert.That(response.WarningsText, Does.StartWith("Already Exists"));
        }
    }
}
