using NUnit.Framework;
using WikiUpload;

namespace Tests.MiscellaneousTests
{
    [TestFixture]
    public class UploadResponseTests
    {
        private readonly string _response01 = @"<?xml version=""1.0""?>
<api>
  <upload result=""Warning"" >
    <warnings duplicate-archive=""Address-book-new.png"" exists=""Address-book-new.png"">
      <duplicate>
        <duplicate>Address-book-new.png</duplicate>
      </duplicate>
    </warnings>
  </upload>
</api>";

        private readonly string _response02 = @"<?xml version=""1.0""?>
<api>
  <upload result=""Warning"">
    <warnings  exists=""Address-book-new.png"">
    </warnings>
  </upload>
</api>";

        [Test]
        public void WarnningsTextPlaceFriendlyShortMessageFirst()
        {
            var response = new UploadResponse(_response01, "");

            Assert.That(response.Warnings.ToString(), Does.StartWith("Already Exists."));
        }

        [Test]
        public void WarningTextContainsFriendyWarning()
        {
            var response = new UploadResponse(_response02, "");

            Assert.That(response.Warnings.ToString(), Is.EqualTo("Already Exists."));
        }
    }
}
