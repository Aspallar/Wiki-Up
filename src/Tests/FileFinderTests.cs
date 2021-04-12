using FakeItEasy;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using WikiUpload;
using WikiUpload.Properties;

namespace Tests
{
    [TestFixture]
    public class FileFinderTests
    {
        private IFileUploader _fileUploader;
        private IHelpers _helpers;
        private IAppSettings _appSettings;
        private FileFinder _finder;
        private IReadOnlyPermittedFiles _permittedFiles;

        [SetUp]
        public void SetUp()
        {
            _permittedFiles = A.Fake<IReadOnlyPermittedFiles>();
            _fileUploader = A.Fake<IFileUploader>();
            _helpers = A.Fake<IHelpers>();
            _appSettings = A.Fake<IAppSettings>();
            _finder = new FileFinder(_fileUploader, _appSettings, _helpers);

            A.CallTo(() => _fileUploader.PermittedFiles)
                .Returns(_permittedFiles);

            A.CallTo(() => _permittedFiles.IsPermitted(A<string>.Ignored))
                .ReturnsLazily((string fileName) =>
                {
                    var extension = Path.GetExtension(fileName);
                    return extension == ".jpg" || extension == ".png" || extension == ".foo";
                });

            A.CallTo(() => _helpers.EnumerateFiles(A<string>._, "*", A<SearchOption>._))
                .Returns(new List<string>
                {
                    "1.jpg",
                    "2.png",
                    "3.foo",
                    "4.bar",
                    "5.zoo"
                });

            A.CallTo(() => _helpers.EnumerateFiles(A<string>._, "*.zoo", A<SearchOption>._))
                .Returns(new List<string> { "5.zoo" });

            A.CallTo(() => _appSettings.ImageExtensions)
                .Returns("jpg;png;zoo");
        }

        [Test]
        public void When_IncludeSubfolders_Then_SubfoldersAreEnumerated()
        {
            _ = _finder.GetFiles("", true, IncludeFiles.All, "");

            A.CallTo(() => _helpers.EnumerateFiles(A<string>._, A<string>._, SearchOption.AllDirectories))
                .MustHaveHappened();
        }

        [Test]
        public void When_NotIncludeSubfolders_Then_SubfoldersAreNotEnumerated()
        {
            _ = _finder.GetFiles("", false, IncludeFiles.All, "");

            A.CallTo(() => _helpers.EnumerateFiles(A<string>._, A<string>._, SearchOption.TopDirectoryOnly))
                .MustHaveHappened();
        }

        [Test]
        public void When_InclideSingleExtension_Then_OnlyFilesWithExtensionAreEnumerated()
        {
            _ = _finder.GetFiles("", false, IncludeFiles.SingleExtension, "foo");

            A.CallTo(() => _helpers.EnumerateFiles(A<string>._, "*.foo", A<SearchOption>._))
                .MustHaveHappened();
        }

        [Test]
        public void When_InclideAll_Then_AllExtensionsAreEnumerated()
        {
            _ = _finder.GetFiles("", false, IncludeFiles.All, "foo");

            A.CallTo(() => _helpers.EnumerateFiles(A<string>._, "*", A<SearchOption>._))
                .MustHaveHappened();
        }

        [Test]
        public void When_IncludeYploadable_Then_AllExtensionsAreEnumerated()
        {
            _ = _finder.GetFiles("", false, IncludeFiles.Uploadable, "foo");

            A.CallTo(() => _helpers.EnumerateFiles(A<string>._, "*", A<SearchOption>._))
                .MustHaveHappened();
        }

        [Test]
        public void When_IncludeImages_Then_AllExtensionsAreEnumerated()
        {
            _ = _finder.GetFiles("", false, IncludeFiles.Image, "foo");

            A.CallTo(() => _helpers.EnumerateFiles(A<string>._, "*", A<SearchOption>._))
                .MustHaveHappened();
        }

        [Test]
        public void When_FilesAreEnumerated_Then_CorrectFolderIsUsed()
        {
            _ = _finder.GetFiles("foobar", false, IncludeFiles.Image, "foo");

            A.CallTo(() => _helpers.EnumerateFiles("foobar", A<string>._, A<SearchOption>._))
                .MustHaveHappened();
        }

        [Test]
        public void When_AllFiles_Then_AllFilesReturned()
        {
            var result = _finder.GetFiles("", false, IncludeFiles.All, ""); ;

            Assert.That(result, Is.EquivalentTo(new List<string> { "1.jpg", "2.png", "3.foo", "4.bar", "5.zoo" }));
        }

        [Test]
        public void When_Uploadable_Then_OnlyUploadableFilesReturned()
        {
            var result = _finder.GetFiles("", false, IncludeFiles.Uploadable, "");

            Assert.That(result, Is.EquivalentTo(new List<string> { "1.jpg", "2.png", "3.foo" }));
        }

        [Test]
        public void When_Image_Then_OnlyUploadableImageFilesReturned()
        {
            var result = _finder.GetFiles("", false, IncludeFiles.Image, "");

            Assert.That(result, Is.EquivalentTo(new List<string> { "1.jpg", "2.png" }));
        }

        [Test]
        public void When_SingleExtension_Then_OnlyFilesWithGivenExtensionReturned()
        {
            var result = _finder.GetFiles("", false, IncludeFiles.SingleExtension, "zoo");

            Assert.That(result, Is.EquivalentTo(new List<string> { "5.zoo" }));
        }

    }
}
