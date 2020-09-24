using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using WikiUpload;

namespace Tests
{
    [TestFixture]
    public class UploadListTests
    {
        [Test]
        public void When_AddRange_Then_FilesAreAdded()
        {
            var uploadList = new UploadList();
            var addFiles = new List<UploadFile>
            {
                new UploadFile() { FullPath = "a"},
                new UploadFile() { FullPath = "b"},
            };

            uploadList.AddRange(addFiles);

            Assert.That(uploadList.ToList(), Is.EqualTo(addFiles));
        }

        [Test]
        public void When_AddNewRange_Then_FilesAreAdded()
        {
            var uploadList = new UploadList();
            var addFiles = new List<string> { "a", "b" };

            uploadList.AddNewRange(addFiles);

            Assert.That(uploadList.Select(x => x.FullPath).ToList(), Is.EqualTo(addFiles));
        }

        [Test]
        public void When_RemoveRange_Then_FilesAreRemoved()
        {
            var file1 = new UploadFile { FullPath = "a" };
            var file2 = new UploadFile { FullPath = "b" };
            var file3 = new UploadFile { FullPath = "c" };
            var uploadList = new UploadList() { file1, file2, file3 };

            uploadList.RemoveRange(new List<UploadFile> { file1, file3 });

            Assert.That(uploadList.Count, Is.EqualTo(1));
            Assert.That(uploadList[0], Is.EqualTo(file2));
        }
    }
}
