using FakeItEasy;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using WikiUpload;
using WikiUpload.Properties;

namespace Tests.ViewModelTests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class AddFolderOptionsViewModelTests
    {
        private IFileUploader _fileUploader;
        private IReadOnlyPermittedFiles _permittedFiles;
        private Window _window;

        [SetUp]
        public void SetUp()
        {
            _fileUploader = A.Fake<IFileUploader>();
            _permittedFiles = A.Fake<IReadOnlyPermittedFiles>();
            A.CallTo(() => _permittedFiles.GetExtensions())
                .Returns(new string [] { ".foo"  });
            A.CallTo(() => _fileUploader.PermittedFiles)
                .Returns(_permittedFiles);
            _window = new Window();
        }

        [Test]
        public void When_Intialized_Then_IncludeFilesOfTypePopulated()
        {
            var expected = new List<string>
            {
                Resources.IncludeAllFiles,
                Resources.IncludeUploadableFiles,
                Resources.IncludeImageFiles,
                "FOO"
            };

            var model = new AddFolderOptionsViewModel(_window, _fileUploader);

            Assert.That(model.FileTypes, Is.EquivalentTo(expected));
        }

        [Test]
        public void When_Intialized_Then_DefaultIncludeIsUploadable()
        {
            var model = new AddFolderOptionsViewModel(_window, _fileUploader);
            var result = model.FileTypes[model.SelectedFileTypeIndex];

            Assert.That(result, Is.EqualTo(Resources.IncludeUploadableFiles));
        }

        [Test]
        public void GetIncludeFiles_Returns_CorrectValue()
        {
            var model = new AddFolderOptionsViewModel(_window, _fileUploader);

            model.SelectedFileTypeIndex = 0;
            Assert.That(model.GetIncludeFiles(), Is.EqualTo(IncludeFiles.All));

            model.SelectedFileTypeIndex = 1;
            Assert.That(model.GetIncludeFiles(), Is.EqualTo(IncludeFiles.Uploadable));

            model.SelectedFileTypeIndex = 2;
            Assert.That(model.GetIncludeFiles(), Is.EqualTo(IncludeFiles.Image));

            model.SelectedFileTypeIndex = 3;
            Assert.That(model.GetIncludeFiles(), Is.EqualTo(IncludeFiles.SingleExtension));
        }

        [Test]
        public void GetExtension_Returns_CorrectValue()
        {
            var model = new AddFolderOptionsViewModel(_window, _fileUploader);

            model.SelectedFileTypeIndex = 0;
            Assert.That(model.GetExtension(), Is.Empty);

            model.SelectedFileTypeIndex = 1;
            Assert.That(model.GetExtension(), Is.Empty);

            model.SelectedFileTypeIndex = 2;
            Assert.That(model.GetExtension(), Is.Empty);

            model.SelectedFileTypeIndex = 3;
            Assert.That(model.GetExtension(), Is.EqualTo("FOO"));
        }

    }
}
