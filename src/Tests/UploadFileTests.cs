using NUnit.Framework;
using WikiUpload;

namespace Tests
{
    [TestFixture]
    public class UploadFFileTests
    {
        [Test]
        public void When_Created_Then_DefaultIsAwaitingUpload()
        {
            var file = new UploadFile();

            Assert.That(file.Status, Is.EqualTo(UploadFileStatus.Waiting));
            Assert.That(file.Message, Is.EqualTo("Awaiting upload."));
        }

        [Test]
        public void SetError_SetsStatusToErrorWithMessage()
        {
            var file = new UploadFile();
            const string message = "foo";

            file.SetError(message);

            Assert.That(file.Status, Is.EqualTo(UploadFileStatus.Error));
            Assert.That(file.Message, Is.EqualTo(message));
        }

        [Test]
        public void SetWarning_SetsStatusToWarningWithMessage()
        {
            var file = new UploadFile();
            const string message = "foo";

            file.SetWarning(message);

            Assert.That(file.Status, Is.EqualTo(UploadFileStatus.Warning));
            Assert.That(file.Message, Is.EqualTo(message));
        }

        [Test]
        public void SetUploiading_SetsStatusToUploading()
        {
            var file = new UploadFile();

            file.SetUploading();

            Assert.That(file.Status, Is.EqualTo(UploadFileStatus.Uploading));
            Assert.That(file.Message, Is.EqualTo("Uploading to wiki..."));
        }

        [Test]
        public void FileName_ReturnsCorrectValue()
        {
            var file = new UploadFile() { FullPath = @"c:\a\b.jpg" };

            Assert.That(file.FileName, Is.EqualTo("b.jpg"));
        }

        [Test]
        public void Folder_ReturnsCorrectValue()
        {
            var file = new UploadFile() { FullPath = @"c:\a\b.jpg" };

            Assert.That(file.Folder, Is.EqualTo(@"c:\a"));
        }

    }
}
