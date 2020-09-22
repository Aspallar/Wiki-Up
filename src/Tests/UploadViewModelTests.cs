using Castle.Components.DictionaryAdapter.Xml;
using Castle.Core.Internal;
using FakeItEasy;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using WikiUpload;

namespace Tests
{
    [TestFixture]
    public class UploadViewModelTests
    {
        private IDialogManager _dialogs;
        private WikiUpload.Properties.IAppSettings _appSetttings;
        private IFileUploader _fileUploader;
        private IHelpers _helpers;
        private IUploadListSerializer _uploadListSerializer;
        private UploadViewModel _model;

        #region Setup
        [SetUp]
        public void Setup()
        {
            _dialogs = A.Fake<IDialogManager>();
            _appSetttings = A.Fake<WikiUpload.Properties.IAppSettings>();
            _fileUploader = A.Fake<IFileUploader>();
            _uploadListSerializer = A.Fake<IUploadListSerializer>();
            _helpers = A.Fake<IHelpers>();

            _model = new UploadViewModel(_fileUploader,
                _dialogs,
                _helpers,
                _uploadListSerializer,
                _appSetttings);
        }

        [Test]
        public void When_aPoliticianIsSpeaking_Then_TheyAreLying()
        {
            Assert.Pass();
        }
        #endregion

        #region Page content Load and Save
        [Test]
        public void When_LoadContentIsExecutedAndFileIsChosen_Then_ContentIsLoadedFromFile()
        {
            const string thePath = "foobar.txt";
            const string content = "Some Content";
            string path;

            A.CallTo(() => _dialogs.LoadContentDialog(out path))
                .Returns(true)
                .AssignsOutAndRefParameters(thePath);
            A.CallTo(() => _helpers.ReadAllText(thePath)).Returns(content);
            _model.PageContent = "";

            _model.LoadContentCommand.Execute(null);

            Assert.That(_model.PageContent, Is.EqualTo(content));
        }

        [Test]
        public void When_LoadContentIsExecutedAndCancelled_Then_ContentIsUnchanged()
        {
            const string newContent = "Some Content";
            const string originalContent = "Initial Content";
            string path;

            A.CallTo(() => _dialogs.LoadContentDialog(out path)).Returns(false);
            A.CallTo(() => _helpers.ReadAllText(A<string>._)).Returns(newContent);
            _model.PageContent = originalContent;

            _model.LoadContentCommand.Execute(null);

            Assert.That(_model.PageContent, Is.EqualTo(originalContent));
        }

        [Test]
        public void When_SaveContentIsExecutedAndFileIsChosen_Then_ContentIsUnchangedAndSavedToFile()
        {
            const string thePath = "foobar.txt";
            const string content = "Some Content";
            string path;

            A.CallTo(() => _dialogs.SaveContentDialog(out path))
                .Returns(true)
                .AssignsOutAndRefParameters(thePath);
            _model.PageContent = content;

            _model.SaveContentCommand.Execute(null);

            Assert.That(_model.PageContent, Is.EqualTo(content));
            A.CallTo(() => _helpers.WriteAllText(thePath, content))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_SaveContentIsExecutedAndCancelled_Then_ContentIsUnchangedAndNotSaved()
        {
            const string content = "Some Content";
            string path;

            A.CallTo(() => _dialogs.SaveContentDialog(out path))
                .Returns(false);

            _model.PageContent = content;

            _model.SaveContentCommand.Execute(null);

            Assert.That(_model.PageContent, Is.EqualTo(content));
            A.CallTo(() => _helpers.WriteAllText(A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void When_PageContentReadError_Then_ErrorIsShown()
        {
            const string thePath = "foobar.txt";
            string path;
            A.CallTo(() => _helpers.ReadAllText(A<string>._))
                .Throws(new IOException());
            A.CallTo(() => _dialogs.LoadContentDialog(out path))
                .Returns(true)
                .AssignsOutAndRefParameters(thePath);

            _model.LoadContentCommand.Execute(null);

            A.CallTo(() => _dialogs.ErrorMessage(A<string>.That.StartsWith("Unable to read file.")))
                .MustHaveHappened(1, Times.Exactly);

        }

        [Test]
        public void When_PageContentWriteError_Then_ErrorIsShown()
        {
            const string thePath = "foobar.txt";
            string path;
            A.CallTo(() => _helpers.WriteAllText(A<string>._, A<string>._))
                .Throws(new IOException());
            A.CallTo(() => _dialogs.SaveContentDialog(out path))
                .Returns(true)
                .AssignsOutAndRefParameters(thePath);

            _model.SaveContentCommand.Execute(null);

            A.CallTo(() => _dialogs.ErrorMessage(A<string>.That.StartsWith("Unable to save file.")))
                .MustHaveHappened(1, Times.Exactly);
        }
        #endregion

        #region Page content Add Category
        private const string ExpectedCategory = "[[Category:Enter Category Name]]";

        [Test]
        public void When_AddCategoryIsExecutedWithEmptyContent_Then_NewCategoryAddedTWithoutNewline()
        {
            _model.PageContent = "";

            _model.AddCategoryCommand.Execute(null);

            Assert.That(_model.PageContent, Is.EqualTo(ExpectedCategory));
        }

        [Test]
        public void When_AddCategoryIsExecutedWithNewlinedContent_Then_NewCategoryAddedTWithoutNewline()
        {
            _model.PageContent = "Foobar\n";
            var expected = _model.PageContent + ExpectedCategory;

            _model.AddCategoryCommand.Execute(null);

            Assert.That(_model.PageContent, Is.EqualTo(expected));
        }

        [Test]
        public void When_AddCategoryIsExecutedWithoutNewlinedContenbt_Then_NewCategoryAddedTWithNewline()
        {
            _model.PageContent = "Foobar";
            var expected = _model.PageContent + "\n" + ExpectedCategory;

            _model.AddCategoryCommand.Execute(null);

            Assert.That(_model.PageContent, Is.EqualTo(expected));
        }

        [Test]
        public void When_AddCategoryIsExecuted_Then_CategpryNamePartIsSelected()
        {
            _model.PageContent = "";

            _model.AddCategoryCommand.Execute(null);

            Assert.That(_model.PageContentSelection, Is.Not.Null);
            Assert.That(_model.PageContentSelection.Start, Is.EqualTo(11));
            Assert.That(_model.PageContentSelection.Length, Is.EqualTo(19));
        }

        [Test]
        public void When_AddCategoryIsExecuted_Then_PageContentSelectionAlwaysChanges()
        {
            _model.PageContent = "";

            _model.AddCategoryCommand.Execute(null);
            var firstSelection = _model.PageContentSelection;
            _model.PageContent = "";
            _model.AddCategoryCommand.Execute(null);

            var isSameReference = Object.ReferenceEquals(_model.PageContentSelection, firstSelection);
            Assert.That(isSameReference, Is.False);
        }

        #endregion

        #region Upload files Load and Save
        [Test]
        public void When_LoadUploadFilesIsExecutedAndFileIsChosen_Then_UploadFilesIsAppenedFromFile()
        {
            string path;
            const string thePath = "foobar.wul";

            A.CallTo(() => _dialogs.LoadUploadListDialog(out path))
                .Returns(true)
                .AssignsOutAndRefParameters(thePath);

            _model.LoadListCommand.Execute(null);

            A.CallTo(() => _uploadListSerializer.Add(thePath, _model.UploadFiles))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_LoadUploadFilesIsExecutedAndCancelled_Then_UploadFilesIsUnchanged()
        {
            string path;

            A.CallTo(() => _dialogs.LoadUploadListDialog(out path))
                .Returns(false);

            _model.LoadListCommand.Execute(null);

            A.CallTo(() => _uploadListSerializer.Add(A<string>._, _model.UploadFiles))
                .MustNotHaveHappened();
            Assert.That(_model.UploadFiles.Count, Is.Zero);
        }

        [Test]
        public void When_SaveUploadFilesIsExecutedAndFileIsChosen_Then_UploadFilesIsSavedToFile()
        {
            string path;
            const string thePath = "foobar.wul";

            A.CallTo(() => _dialogs.SaveUploadListDialog(out path))
                .Returns(true)
                .AssignsOutAndRefParameters(thePath);

            _model.SaveListCommand.Execute(null);

            A.CallTo(() => _uploadListSerializer.Save(thePath, _model.UploadFiles))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_SaveUploadFilesIsExecutedAndCancelled_Then_UploadFilesNotSaved()
        {
            string path;

            A.CallTo(() => _dialogs.SaveUploadListDialog(out path))
                .Returns(false);

            _model.SaveListCommand.Execute(null);

            A.CallTo(() => _uploadListSerializer.Save(A<string>._, _model.UploadFiles))
                .MustNotHaveHappened();
        }

        [Test]
        public void When_LoadUploadFilesReadError_Then_ErrorMessageIsShown()
        {
            string path;
            const string thePath = "foobar.wul";

            A.CallTo(() => _uploadListSerializer.Add(A<string>._, A<UploadList>._))
                .Throws(new IOException());
            A.CallTo(() => _dialogs.LoadUploadListDialog(out path))
                .Returns(true)
                .AssignsOutAndRefParameters(thePath);

            _model.LoadListCommand.Execute(null);

            A.CallTo(() => _dialogs.ErrorMessage(A<string>.That.StartsWith("Unable to read upload list.")))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_LoadUploadFilesFormatError_Then_ErrorMessageIsShown()
        {
            string path;
            const string thePath = "foobar.wul";

            A.CallTo(() => _uploadListSerializer.Add(A<string>._, A<UploadList>._))
                .Throws(new InvalidOperationException());
            A.CallTo(() => _dialogs.LoadUploadListDialog(out path))
                .Returns(true)
                .AssignsOutAndRefParameters(thePath);

            _model.LoadListCommand.Execute(null);

            A.CallTo(() => _dialogs.ErrorMessage("Invalid file format."))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_SaveUploadFilesWriteError_Then_ErrorMessageIsShown()
        {
            string path;
            const string thePath = "foobar.wul";

            A.CallTo(() => _uploadListSerializer.Save(A<string>._, A<UploadList>._))
                .Throws(new IOException());
            A.CallTo(() => _dialogs.SaveUploadListDialog(out path))
                .Returns(true)
                .AssignsOutAndRefParameters(thePath);

            _model.SaveListCommand.Execute(null);

            A.CallTo(() => _dialogs.ErrorMessage(A<string>.That.StartsWith("Unable to save file.")))
                .MustHaveHappened(1, Times.Exactly);
        }


        #endregion

        #region Upload Files Add and Remove 
        [Test]
        public void When_AddFilesIsExecutedAndFilesChosen_Then_FilesAreAddedToUploadFiles()
        {
            IList<string> list;
            const string file1 = "foobar.jpg";
            const string file2 = "foo.jpg";
            const string file3 = "bar.jpg";

            _model.UploadFiles.Add(new UploadFile { FullPath = file1 });
            A.CallTo(() => _dialogs.AddFilesDialog(A<string[]>._, A<string>._, out list))
                .Returns(true)
                .AssignsOutAndRefParameters(new List<string> { file2, file3 });

            _model.AddFilesCommand.Execute(null);

            Assert.That(_model.UploadFiles.Count, Is.EqualTo(3));
            Assert.That(_model.UploadFiles.Any(x => x.FullPath == file1), Is.True);
            Assert.That(_model.UploadFiles.Any(x => x.FullPath == file2), Is.True);
            Assert.That(_model.UploadFiles.Any(x => x.FullPath == file3), Is.True);
        }

        [Test]
        public void When_AddFilesIsExecutedAndCancelled_Then_UploadFilesIsUnchanged()
        {
            IList<string> list;

            A.CallTo(() => _dialogs.AddFilesDialog(A<string[]>._, A<string>._, out list))
                .Returns(false)
                .AssignsOutAndRefParameters(new List<string> { "foo" });

            _model.AddFilesCommand.Execute(null);

            Assert.That(_model.UploadFiles.Count, Is.Zero);
        }

        [Test]
        public void When_RemoveFilesIsExecuted_Then_FilesAreRemovedFromUploadFiles()
        {
            const string fileName = "foobar.jpg";
            var removeFiles = new List<UploadFile>
            {
                new UploadFile { FullPath = "foo.jpg" },
                new UploadFile { FullPath = "bar.jpg" },
            };
            _model.UploadFiles.Add(new UploadFile { FullPath = fileName });
            _model.UploadFiles.AddRange(removeFiles);

            _model.RemoveFilesCommand.Execute(removeFiles);

            Assert.That(_model.UploadFiles.Count, Is.EqualTo(1));
            Assert.That(_model.UploadFiles[0].FullPath, Is.EqualTo(fileName));
        }

        [Test]
        public void When_RemoveFilesIsExecutedAndUploadIsRunning_Then_FilesAreNotRemovedFromUploadFiles()
        {
            const string file1 = "foo.jpg";
            const string file2 = "bar.jpg";
            var removeFiles = new List<UploadFile>
            {
                new UploadFile { FullPath = file1 },
                new UploadFile { FullPath = file2 },
            };
            _model.UploadFiles.AddRange(removeFiles);
            _model.UploadIsRunning = true;

            _model.RemoveFilesCommand.Execute(removeFiles);

            Assert.That(_model.UploadFiles.Count, Is.EqualTo(2));
            Assert.That(_model.UploadFiles.Any(x => x.FullPath == file1), Is.True);
            Assert.That(_model.UploadFiles.Any(x => x.FullPath == file2), Is.True);
        }

        #endregion

        #region Process Launch
        [Test]
        public void When_SiteNameIsExecuted_Then_SiteIsLaunched()
        {
            const string siteUrl = "https://foobar.com";
            _model.Site = siteUrl;

            _model.LaunchSiteCommand.Execute(null);

            A.CallTo(() => _helpers.LaunchProcess(siteUrl))
                .MustHaveHappened(1, Times.Exactly);
        }

        public void When_ShowFileIsExecuted_Then_FileIsLaunched()
        {
            const string filename = "foo.jpg";

            _model.ShowFileCommand.Execute(filename);

            A.CallTo(() => _helpers.LaunchProcess(filename))
                .MustHaveHappened(1, Times.Exactly);
        }


        #endregion
    }
}
