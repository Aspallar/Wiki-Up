using FakeItEasy;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WikiUpload;

namespace Tests
{
    [TestFixture]
    public class UploadListTests
    {
        private IHelpers _helpers;

        [SetUp]
        public void SetUp()
        {
            _helpers = A.Fake<IHelpers>();
        }

        [Test]
        public void AddAddIfNotDuplicate_AddsFile()
        {
            var uploadList = new UploadList(_helpers);
            var file = new UploadFile { FullPath = "foo.jpg" };

            uploadList.AddIfNotDuplicate(file);

            Assert.That(uploadList.Count, Is.EqualTo(1));
            Assert.That(uploadList[0], Is.EqualTo(file));
        }

        [Test]
        public void Add_WithDuplicate_DoesNotAddFile()
        {
            var uploadList = new UploadList(_helpers);
            var file1 = new UploadFile { FullPath = "a" };
            var file2 = new UploadFile { FullPath = "a" };

            uploadList.AddIfNotDuplicate(file1);
            uploadList.AddIfNotDuplicate(file2);

            Assert.That(uploadList.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddRange_FilesAreAdded()
        {
            var uploadList = new UploadList(_helpers);
            var addFiles = new List<UploadFile>
            {
                new UploadFile() { FullPath = "a"},
                new UploadFile() { FullPath = "b"},
            };

            uploadList.AddRange(addFiles);

            Assert.That(uploadList.ToList(), Is.EqualTo(addFiles));
        }

        [Test]
        public async Task AddRangeAsync_FilesAreAdded()
        {
            var uploadList = new UploadList(_helpers);
            var addFiles = new List<UploadFile>();

            for (var k = 0; k < 500; k++)
                addFiles.Add(new UploadFile(k.ToString()));

            await uploadList.AddRangeAsync(addFiles);

            Assert.That(uploadList.ToList(), Is.EqualTo(addFiles));
        }

        [Test]
        public void AddRange_WithDuplicate_DuplicatesAreNotAdded()
        {
            var uploadList = new UploadList(_helpers);
            const string duplicatePath = "a";
            var duplicate = new UploadFile() { FullPath = duplicatePath };
            var addFiles1 = new List<UploadFile>
            {
                new UploadFile() { FullPath = duplicatePath },
                new UploadFile() { FullPath = "b"},
            };
            var addFiles2 = new List<UploadFile>
            {
                duplicate,
                new UploadFile() { FullPath = "c" },
            };

            uploadList.AddRange(addFiles1);
            uploadList.AddRange(addFiles2);

            Assert.That(uploadList.Count, Is.EqualTo(3));
            Assert.That(uploadList, Does.Not.Contain(duplicate));
        }

        [Test]
        public async Task AddRangeAsync_WithDuplicate_DuplicatesAreNotAdded()
        {
            var uploadList = new UploadList(_helpers);
            const string duplicatePath = "a";
            var duplicate = new UploadFile() { FullPath = duplicatePath };
            var addFiles1 = new List<UploadFile>
            {
                new UploadFile() { FullPath = duplicatePath },
                new UploadFile() { FullPath = "b"},
            };
            var addFiles2 = new List<UploadFile>
            {
                duplicate,
                new UploadFile() { FullPath = "c" },
            };

            uploadList.AddRange(addFiles1);
            await uploadList.AddRangeAsync (addFiles2);

            Assert.That(uploadList.Count, Is.EqualTo(3));
            Assert.That(uploadList, Does.Not.Contain(duplicate));
        }

        [Test]
        public void AddNewRange_FilesAreAdded()
        {
            var uploadList = new UploadList(_helpers);
            var addFiles = new List<string> { "a", "b" };

            uploadList.AddNewRange(addFiles);

            Assert.That(uploadList.Select(x => x.FullPath).ToList(), Is.EqualTo(addFiles));
        }

        [Test]
        public async Task AddNewRangeAsync_FilesAreAdded()
        {
            var uploadList = new UploadList(_helpers);
            var addFiles = new List<string>();

            for (var k = 0; k < 500; k++)
                addFiles.Add(k.ToString());

            await uploadList.AddNewRangeAsync(addFiles);

            Assert.That(uploadList.Select(x => x.FullPath).ToList(), Is.EqualTo(addFiles));
        }

        [Test]
        public void AddNewRange_WithDuplicates_DuplicatesAreNotAdded()
        {
            const string duplicatePath = "a";
            var uploadList = new UploadList(_helpers)
            {
                new UploadFile { FullPath = duplicatePath },
                new UploadFile { FullPath = "b" },
            };
            
            var addFiles = new List<string> { duplicatePath, "c" };

            uploadList.AddNewRange(addFiles);

            Assert.That(uploadList.Count, Is.EqualTo(3));
            Assert.That(uploadList.Count(x => x.FullPath == duplicatePath), Is.EqualTo(1));
        }

        [Test]
        public async Task AddNewRangeAsync_WithDuplicates_DuplicatesAreNotAdded()
        {
            const string duplicatePath = "a";
            var uploadList = new UploadList(_helpers)
            {
                new UploadFile { FullPath = duplicatePath },
                new UploadFile { FullPath = "b" },
            };
            
            var addFiles = new List<string> { duplicatePath, "c" };

            await uploadList.AddNewRangeAsync(addFiles);

            Assert.That(uploadList.Count, Is.EqualTo(3));
            Assert.That(uploadList.Count(x => x.FullPath == duplicatePath), Is.EqualTo(1));
        }

        [Test]
        public void When_RemoveRange_Then_FilesAreRemoved()
        {
            var file1 = new UploadFile { FullPath = "a" };
            var file2 = new UploadFile { FullPath = "b" };
            var file3 = new UploadFile { FullPath = "c" };
            var uploadList = new UploadList(_helpers) { file1, file2, file3 };

            uploadList.RemoveRange(new List<UploadFile> { file1, file3 });

            Assert.That(uploadList.Count, Is.EqualTo(1));
            Assert.That(uploadList[0], Is.EqualTo(file2));
        }
    }
}
