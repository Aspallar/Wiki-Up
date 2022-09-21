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
        public void When_FullPathIsSet_Then_UploadFileName_IsSameAsFilename()
        {
            var file = new UploadFile();
            file.FullPath = @"a:\b\foo.jpg";

            Assert.That(file.UploadFileName, Is.EqualTo("foo.jpg"));
        }


        [Test]
        public void When_UploadFileNameHasValue_Then_UploadFileName_Returns_FileName()
        {
            var file = new UploadFile();
            file.FullPath = @"a:\b\foo.jpg";
            file.UploadFileName = "bar.png";

            Assert.That(file.UploadFileName, Is.EqualTo("bar.png"));
        }


        [Test]
        public void When_UploadFileNameIsNull_Then_UploadFileName_Returns_FileName()
        {
            var file = new UploadFile();
            file.FullPath = @"a:\b\foo.jpg";
            file.UploadFileName = null;

            Assert.That(file.UploadFileName, Is.EqualTo("foo.jpg"));
        }

        [Test]
        public void When_UploadFileNameIsEmpty_Then_UploadFileName_Returns_FileName()
        {
            var file = new UploadFile();
            file.FullPath = @"a:\b\foo.jpg";
            file.UploadFileName = "";

            Assert.That(file.UploadFileName, Is.EqualTo("foo.jpg"));
        }

        [Test]
        public void When_UploadFileNameIsWhitespace_Then_UploadFileName_Returns_FileName()
        {
            var file = new UploadFile();
            file.FullPath = @"a:\b\foo.jpg";
            file.UploadFileName = "     ";

            Assert.That(file.UploadFileName, Is.EqualTo("foo.jpg"));
        }

        [Test]
        public void When_UploadFileName_IsSameAsFilename_Then_DisplayNameIsFilenameOnly()
        {
            var file = new UploadFile();
            file.FullPath = @"a:\b\foo.jpg";
            file.UploadFileName = "foo.jpg";

            Assert.That(file.DisplayName, Is.EqualTo("foo.jpg"));
        }

        [Test]
        public void When_UploadFileName_IsEmpty_Then_DisplayNameIsFilenameOnly()
        {
            var emptyNames = new string[] { null, "", "  " };
            var file = new UploadFile();
            file.FullPath = @"a:\b\foo.jpg";

            foreach (var name in emptyNames)
            {
                file.UploadFileName = name;
                Assert.That(file.DisplayName, Is.EqualTo("foo.jpg"));
            }
        }

        [Test]
        public void When_UploadFileName_IsnOTSameAsFilename_Then_DisplayNameContainsBothNames()
        {
            var file = new UploadFile();
            file.FullPath = @"a:\b\foo.jpg";
            file.UploadFileName = "bar.jpg";

            Assert.That(file.DisplayName, Does.Contain("foo.jpg"));
            Assert.That(file.DisplayName, Does.Contain("bar.jpg"));
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
