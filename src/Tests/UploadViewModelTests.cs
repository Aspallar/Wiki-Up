using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
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
        private IUploadResponse _uploadResponse;
        private INavigatorService _navigationService;
        private IWikiSearchFactory _wikiSearchFactory;
        private IWikiSearch _categorySearch;
        private IWikiSearch _templateSearch;
        private IYoutube _youtube;
        private IUploadListSerializer _uploadListSerializer;
        private IReadOnlyPermittedFiles _permittedFiles;
        private UploadViewModel _model;

        #region Setup
        [SetUp]
        public void Setup()
        {
            _permittedFiles = A.Fake<IReadOnlyPermittedFiles>();
            _dialogs = A.Fake<IDialogManager>();
            _appSetttings = A.Fake<WikiUpload.Properties.IAppSettings>();
            _fileUploader = A.Fake<IFileUploader>();
            _uploadListSerializer = A.Fake<IUploadListSerializer>();
            _helpers = A.Fake<IHelpers>();
            _uploadResponse = A.Fake<IUploadResponse>();
            _navigationService = A.Fake<INavigatorService>();
            _wikiSearchFactory = A.Fake<IWikiSearchFactory>();
            _categorySearch = A.Fake<IWikiSearch>();
            _templateSearch = A.Fake<IWikiSearch>();
            _youtube = A.Fake<IYoutube>();

            A.CallTo(() => _fileUploader.PermittedFiles)
                .Returns(_permittedFiles);

            A.CallTo(() => _wikiSearchFactory.CreateCategorySearch(A<IFileUploader>._))
                .Returns(_categorySearch);
            A.CallTo(() => _wikiSearchFactory.CreateTemplateSearch(A<IFileUploader>._))
                .Returns(_templateSearch);

            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, A<bool>._, A<bool>._))
                .Returns(_uploadResponse);

            _model = new UploadViewModel(_fileUploader,
                _dialogs,
                _helpers,
                _uploadListSerializer,
                _navigationService,
                _wikiSearchFactory,
                _youtube, 
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
                .Throws(new Exception());
            A.CallTo(() => _dialogs.LoadContentDialog(out path))
                .Returns(true)
                .AssignsOutAndRefParameters(thePath);

            _model.LoadContentCommand.Execute(null);

            A.CallTo(() => _dialogs.ErrorMessage("Unable to read content.", A<Exception>._))
                .MustHaveHappened(1, Times.Exactly);

        }

        [Test]
        public void When_PageContentWriteError_Then_ErrorIsShown()
        {
            const string thePath = "foobar.txt";
            string path;
            A.CallTo(() => _helpers.WriteAllText(A<string>._, A<string>._))
                .Throws(new Exception());
            A.CallTo(() => _dialogs.SaveContentDialog(out path))
                .Returns(true)
                .AssignsOutAndRefParameters(thePath);

            _model.SaveContentCommand.Execute(null);

            A.CallTo(() => _dialogs.ErrorMessage(A<string>.That.StartsWith("Unable to save content."), A<Exception>._))
                .MustHaveHappened(1, Times.Exactly);
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
                .Throws(new Exception());
            A.CallTo(() => _dialogs.LoadUploadListDialog(out path))
                .Returns(true)
                .AssignsOutAndRefParameters(thePath);

            _model.LoadListCommand.Execute(null);

            A.CallTo(() => _dialogs.ErrorMessage("Unable to read upload list.", A<Exception>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_SaveUploadFilesWriteError_Then_ErrorMessageIsShown()
        {
            string path;
            const string thePath = "foobar.wul";

            A.CallTo(() => _uploadListSerializer.Save(A<string>._, A<UploadList>._))
                .Throws(new Exception());
            A.CallTo(() => _dialogs.SaveUploadListDialog(out path))
                .Returns(true)
                .AssignsOutAndRefParameters(thePath);

            _model.SaveListCommand.Execute(null);

            A.CallTo(() => _dialogs.ErrorMessage("Unable to save upload list.", A<Exception>._))
                .MustHaveHappened(1, Times.Exactly);
        }
        #endregion

        #region Upload Files Add, Remove abd Drop
        [Test]
        public void When_AddFilesIsExecutedAndFilesChosen_Then_FilesAreAddedToUploadFiles()
        {
            IList<string> list;
            const string file1 = "foobar.jpg";
            const string file2 = "foo.jpg";
            const string file3 = "bar.jpg";

            _model.UploadFiles.AddIfNotDuplicate(new UploadFile { FullPath = file1 });
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
        public void When_AddFilesIsExecutedAndFilesChosen_Then_DuplicateFilesAreANotddedToUploadFiles()
        {
            IList<string> list;
            const string file1 = "foobar.jpg";
            const string file2 = "foobar.jpg";
            const string file3 = "bar.jpg";

            _model.UploadFiles.AddIfNotDuplicate(new UploadFile { FullPath = file1 });
            A.CallTo(() => _dialogs.AddFilesDialog(A<string[]>._, A<string>._, out list))
                .Returns(true)
                .AssignsOutAndRefParameters(new List<string> { file2, file3 });

            _model.AddFilesCommand.Execute(null);

            Assert.That(_model.UploadFiles.Count(x => x.FullPath == file1), Is.EqualTo(1));
            Assert.That(_model.UploadFiles.Count, Is.EqualTo(2));
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
            _model.UploadFiles.AddIfNotDuplicate(new UploadFile { FullPath = fileName });
            _model.UploadFiles.AddRange(removeFiles);

            _model.RemoveFilesCommand.Execute(removeFiles);

            Assert.That(_model.UploadFiles.Count, Is.EqualTo(1));
            Assert.That(_model.UploadFiles[0].FullPath, Is.EqualTo(fileName));
        }

        [Test]
        public void When_RemoveFilesIsExecutedAndUploadIsRunning_Then_FilesAreNotRemovedFromUploadFiles()
        {
            var file1 = new UploadFile { FullPath = "foo.jpg" };
            var file2 = new UploadFile { FullPath = "bar.jpg" };
            var removeFiles = new List<UploadFile> { file1, file2 };
            _model.UploadFiles.AddRange(removeFiles);
            _model.UploadIsRunning = true;

            _model.RemoveFilesCommand.Execute(removeFiles);

            Assert.That(_model.UploadFiles.Count, Is.EqualTo(2));
            Assert.That(_model.UploadFiles, Does.Contain(file1));
            Assert.That(_model.UploadFiles, Does.Contain(file2));
        }

        [Test]
        public void When_FilesAreDropped_Then_DroppedFilesAddedToUploadFiles()
        {
            AddSingleUploadFile();
            const string file1 = "baz.jpg";
            const string file2 = "foo.jpg";
            var dropFiles = new string[] { file1, file2 };

            _model.OnFileDrop(dropFiles, false);

            Assert.That(_model.UploadFiles.Count, Is.EqualTo(dropFiles.Length + 1));
            Assert.That(_model.UploadFiles.Any(x => x.FullPath == file1), Is.True);
            Assert.That(_model.UploadFiles.Any(x => x.FullPath == file2), Is.True);
        }

        [Test]
        public void When_FilesAreDropped_Then_DuplicateFilesAreNotAddedToUploadFiles()
        {
            var file = AddSingleUploadFile();
            var file1 = file.FullPath;
            const string file2 = "foo.jpg";
            var dropFiles = new string[] { file1, file2 };

            _model.OnFileDrop(dropFiles, false);

            Assert.That(_model.UploadFiles.Count(x => x.FullPath == file1), Is.EqualTo(1));
            Assert.That(_model.UploadFiles.Count, Is.EqualTo(2));
        }

        [Test]
        public void When_FilesAreDroppedAndUploadRunning_Then_DroppedFilesNotAddedToUploadFiles()
        {
            var dropFiles = new string[] { "foo.jpg" };
            _model.UploadIsRunning = true;

            _model.OnFileDrop(dropFiles, false);

            Assert.That(_model.UploadFiles.Count, Is.Zero);
        }

        #endregion

        #region Process Launch
        [Test]
        public void When_SiteEndsWithScrtppath_Then_SiteWithoutScriptpathIsDisplayed()
        {
            const string site = "foobar";
            const string scriptpath = "/w";
            A.CallTo(() => _fileUploader.Site)
                .Returns(site + scriptpath);
            A.CallTo(() => _fileUploader.ScriptPath)
                .Returns(scriptpath);

            Assert.That(_model.Site, Is.EqualTo(site));
       }

        [Test]
        public void When_SiteNameIsExecuted_Then_SiteHomePageIsLaunched()
        {
            const string homePage = "foobar";
            A.CallTo(() => _fileUploader.HomePage)
                .Returns(homePage);

            _model.LaunchSiteCommand.Execute(null);

            A.CallTo(() => _helpers.LaunchProcess(homePage))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_ShowFileIsExecuted_Then_FileIsLaunched()
        {
            const string filename = "foo.jpg";

            _model.ShowFileCommand.Execute(filename);

            A.CallTo(() => _helpers.LaunchProcess(filename))
                .MustHaveHappened(1, Times.Exactly);
        }


        #endregion

        #region Upload

        private UploadFile AddSingleUploadFile()
        {
            var file = new UploadFile { FullPath = "Foobar.jpg" };
            _model.UploadFiles.AddIfNotDuplicate(file);
            return file;
        }

        private void AddThreeUploadFiles()
        {
            for (int i = 0; i < 3; i++)
                _model.UploadFiles.AddIfNotDuplicate(new UploadFile() { FullPath = $"BazFoo{i}.jpg" });
        }

        private void AlllFilesPermitted()
        {
            A.CallTo(() => _permittedFiles.IsPermitted(A<string>._)).Returns(true);
        }

        [Test]
        public void When_NothingToUploade_Then_NoUploadIsAttempted()
        {
            AlllFilesPermitted();
            
            _model.UploadCommand.Execute(null);

            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, A<bool>._, A<bool>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void When_UploadIsDone_Then_PageContentIsSet()
        {
            AlllFilesPermitted();
            AddSingleUploadFile();
            A.CallTo(() => _uploadResponse.Result).Returns(ResponseCodes.Success);
            const string content = "foobar";
            _model.PageContent = content;

            _model.UploadCommand.Execute(null);

            A.CallToSet(() => _fileUploader.PageContent).To(content)
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_UploadIsDone_Then_SummaryIsSetWithViaWikiUp()
        {
            AlllFilesPermitted();
            AddSingleUploadFile();
            A.CallTo(() => _uploadResponse.Result).Returns(ResponseCodes.Success);
            const string summary = "foobar";
            _model.UploadSummary = summary;

            _model.UploadCommand.Execute(null);

            A.CallToSet(() => _fileUploader.Summary).To(()=>A<string>.That.StartsWith(summary))
                .MustHaveHappened(1, Times.Exactly);
            A.CallToSet(() => _fileUploader.Summary).To(() => A<string>.That.Contains("via Wiki-Up"))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_UploadIsDoneToFandom_Then_SummaryIsSetWithLinkToDev()
        {
            AlllFilesPermitted();
            AddSingleUploadFile();
            A.CallTo(() => _uploadResponse.Result).Returns(ResponseCodes.Success);
            const string summary = "foobar";
            _model.UploadSummary = summary;

            A.CallTo(() => _fileUploader.Site)
                .Returns(".fandom.");

            _model.UploadCommand.Execute(null);

            A.CallToSet(() => _fileUploader.Summary).To(() => A<string>.That.StartsWith(summary))
                .MustHaveHappened(1, Times.Exactly);
            A.CallToSet(() => _fileUploader.Summary).To(() => A<string>.That.Contains("[[w:c:dev:Wiki-Up|Wiki-Up]]"))
                .MustHaveHappened(1, Times.Exactly);
        }


        [Test]
        public void When_UploadIsSucessfull_Then_FilesAreRemovedFromUploadList()
        {
            AlllFilesPermitted();
            AddSingleUploadFile();
            A.CallTo(() => _uploadResponse.Result).Returns(ResponseCodes.Success);

            _model.UploadCommand.Execute(null);

            Assert.That(_model.UploadFiles.Count, Is.Zero);
        }

        [Test]
        public void When_UploadResultIsError_Then_FilesStatusIsSetToErrorWithMessage()
        {
            AlllFilesPermitted();
            AddSingleUploadFile();

            A.CallTo(() => _uploadResponse.IsError).Returns(true);
            A.CallTo(() => _uploadResponse.IsTokenError).Returns(false);
            const string errorText = "foobar";
            A.CallTo(() => _uploadResponse.ErrorsText).Returns(errorText);

            _model.UploadCommand.Execute(null);

            Assert.That(_model.UploadFiles.Count, Is.EqualTo(1));
            var file = _model.UploadFiles[0];
            Assert.That(file.Status, Is.EqualTo(UploadFileStatus.Error));
            Assert.That(file.Message, Is.EqualTo(errorText));
        }

        [Test]
        public void When_UploadResultIsWarning_Then_FilesStatusIsSetToWarningWithMessage()
        {
            AlllFilesPermitted();
            AddSingleUploadFile();

            A.CallTo(() => _uploadResponse.Result).Returns(ResponseCodes.Warning);
            const string warningText = "foobar";
            A.CallTo(() => _uploadResponse.WarningsText).Returns(warningText);

            _model.UploadCommand.Execute(null);

            Assert.That(_model.UploadFiles.Count, Is.EqualTo(1));
            var file = _model.UploadFiles[0];
            Assert.That(file.Status, Is.EqualTo(UploadFileStatus.Warning));
            Assert.That(file.Message, Is.EqualTo(warningText));
        }

        [Test]
        public void When_FileIsNotPermitted_Then_NoUploadAttempted()
        {
            AddSingleUploadFile();

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, A<bool>._, A<bool>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void When_FileIsNotPermitted_Then_StatusIsSetToErrorWithMessage()
        {
            AddSingleUploadFile();

            _model.UploadCommand.Execute(null);

            Assert.That(_model.UploadFiles.Count, Is.EqualTo(1));
            var file = _model.UploadFiles[0];
            Assert.That(file.Status, Is.EqualTo(UploadFileStatus.Error));
            Assert.That(file.Message, Does.EndWith("are not permitted."));
        }

        [Test]
        public void When_Uploading_Then_StatusIsSetToUploadingWithMessage()
        {
            AlllFilesPermitted();
            var foo = AddSingleUploadFile();
            bool wasSetToUnloading = false;
            foo.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(foo.Status) && foo.Status == UploadFileStatus.Uploading)
                    wasSetToUnloading = true; ;
            };

            _model.UploadCommand.Execute(null);

            Assert.That(wasSetToUnloading, Is.True);
        }

        [Test]
        public void When_Uploading_Then_UploadIsRunningIsSeToTrue()
        {
            AlllFilesPermitted();
            AddSingleUploadFile();
            var wasSetToTrue = false;
            _model.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_model.UploadIsRunning) && _model.UploadIsRunning)
                    wasSetToTrue = true;
            };

            _model.UploadCommand.Execute(null);

            Assert.That(wasSetToTrue, Is.True);
        }

        [Test]
        public void When_ForceUploadsIsOn_Then_UploadIsDoneWithIgnoreWarnings()
        {
            AlllFilesPermitted();
            AddThreeUploadFiles();
            _model.ForceUpload = true;

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, true, A<bool>._))
                .MustHaveHappened();
            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, false, A<bool>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void When_ForceUploadsIsOff_Then_UploadIsDoneWithoutIgnoreWarnings()
        {
            AlllFilesPermitted();
            AddThreeUploadFiles();
            _model.ForceUpload = false;

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, false, A<bool>._))
                .MustHaveHappened();
            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, true, A<bool>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void When_IncludeInWatchlistIsOn_Then_UploadIsDoneWithIncludeInWatchlist()
        {
            AlllFilesPermitted();
            AddThreeUploadFiles();
            _model.IncludeInWatchlist = true;

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, A<bool>._, true))
                .MustHaveHappened();
            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, A<bool>._, false))
                .MustNotHaveHappened();
        }

        [Test]
        public void When_IncludeInWatchlistIsOff_Then_UploadIsDoneWithoutIncludeInWatchlist()
        {
            AlllFilesPermitted();
            AddThreeUploadFiles();
            _model.IncludeInWatchlist = false;

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, A<bool>._, false))
                .MustHaveHappened();
            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, A<bool>._, true))
                .MustNotHaveHappened();
        }

        [Test]
        public void When_Uploading_Then_UploadingFileIsBroughtIntoView()
        {
            AlllFilesPermitted();
            var files = new List<UploadFile>
            {
                new UploadFile { FullPath = "foo.jpg" },
                new UploadFile { FullPath = "bar.jpg" },
                new UploadFile { FullPath = "baz.jpg" },
            };
            _model.UploadFiles.AddRange(files);
            var viewdFiles = new List<UploadFile>();
            _model.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_model.ViewedFile) && _model.ViewedFile != null)
                    viewdFiles.Add(_model.ViewedFile);
            };

            _model.UploadCommand.Execute(null);

            Assert.That(viewdFiles, Is.EqualTo(files));
        }

        [Test]
        public void When_InvalidTokenResponse_Then_OneAttemptIsMadeToRefreshToken()
        {
            AlllFilesPermitted();
            AddThreeUploadFiles();
            A.CallTo(() => _uploadResponse.IsError).Returns(true);
            A.CallTo(() => _uploadResponse.IsTokenError).Returns(true);

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _fileUploader.RefreshTokenAsync())
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_RefreshTokenIsSuccessfull_Then_UploadContinues()
        {
            AlllFilesPermitted();
            AddThreeUploadFiles();
            A.CallTo(() => _uploadResponse.Result)
                .Returns("").Once().Then.Returns(ResponseCodes.Success);
            A.CallTo(() => _uploadResponse.IsError)
                .Returns(true).Once().Then.Returns(false);
            A.CallTo(() => _uploadResponse.IsTokenError)
                .Returns(true).Once().Then.Returns(false);

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _fileUploader.RefreshTokenAsync())
                .MustHaveHappened(1, Times.Exactly);
            Assert.That(_model.UploadFiles.Count, Is.Zero);
        }

        [Test]
        public void When_UnknownResponse_Then_ErrorIsShownAndUploadContinues()
        {
            AlllFilesPermitted();
            AddThreeUploadFiles();
            A.CallTo(() => _uploadResponse.Result).Returns("Foobar");
            A.CallTo(() => _uploadResponse.IsError).Returns(false);
            
            _model.UploadCommand.Execute(null);

            Assert.That(_model.UploadFiles.Count(x => x.Status == UploadFileStatus.Error 
                && x.Message == UploadMessages.UnkownServerResponse), Is.EqualTo(3));
        }

        [Test]
        public void When_CancelExecuted_Then_CancelUploadIsRequested()
        {
            _model.CancelCommand.Execute(null);

            A.CallTo(() => _helpers.SignalCancel(A<CancellationTokenSource>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        #endregion

        #region Upload - Exception Errors

        private void ExceptionErrorTest(Exception errorException, string expedtedMessage, bool stopsUpload = false)
        {
            AlllFilesPermitted();
            AddThreeUploadFiles();
            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, A<bool>._,  A<bool>._))
                .Throws(errorException);

            _model.UploadCommand.Execute(null);

            Assert.That(_model.UploadFiles.Count, Is.EqualTo(3));
            var file = _model.UploadFiles[0];
            Assert.That(file.Status, Is.EqualTo(UploadFileStatus.Error));
            Assert.That(file.Message, Is.EqualTo(expedtedMessage));

            if (stopsUpload)
            {
                Assert.That(_model.UploadFiles.Count(x => x.Status == UploadFileStatus.Error), Is.EqualTo(1),
                    "Upload should have stopped after error but it continued.");
            }
            else
            {
                Assert.That(_model.UploadFiles.Count(x => x.Status == UploadFileStatus.Error), Is.EqualTo(3),
                    "Upload should have continued after error but it stopped.");
            }
        }

        [Test]
        public void WhenUploadThrows_XmlException_Then_ErrorIsShown()
            => ExceptionErrorTest(new XmlException(), UploadMessages.InvalidXml);

        [Test]
        public void WhenUploadThrows_FileNotFoundException_Then_ErrorIsShown()
            => ExceptionErrorTest(new FileNotFoundException(), UploadMessages.FileNotFound);

        [Test]
        public void WhenUploadThrows_IOException_Then_ErrorIsShown()
            => ExceptionErrorTest(new IOException(), UploadMessages.ReadFail);

        [Test]
        public void WhenUploadThrows_OperationCanceledException_Then_ErrorIsShownAndUploadStopped()
            => ExceptionErrorTest(new OperationCanceledException(), UploadMessages.Cancelled, true);

        [Test]
        public void WhenUploadThrows_ServerIsBusyException_Then_ErrorIsShownAndUploadStopped()
            => ExceptionErrorTest(new ServerIsBusyException(), UploadMessages.ServerBusy, true);

        [Test]
        public void WhenUploadThrows_NoEditTokenException_Then_ErrorIsShownAndUploadStopped()
            => ExceptionErrorTest(new NoEditTokenException(), UploadMessages.NoEditToken, true);

        [Test]
        public void WhenUploadThrows_HttpRequestException_Then_ErrorIsShown()
            => ExceptionErrorTest(new HttpRequestException(), UploadMessages.NetworkError);

        [Test]
        public void WhenUploadThrows_HttpRequestException_With_IOException_Then_ErrorIsShown()
            => ExceptionErrorTest(new HttpRequestException("foobar", new IOException()), UploadMessages.ReadFail);

        [Test]
        public void WhenUploadThrows_TaskCanceledException_DueToTimeout_Then_ErrorIsShown()
        {
            A.CallTo(() => _helpers.IsCancellationRequested(A<CancellationToken>._)).Returns(false);
            ExceptionErrorTest(new TaskCanceledException(), UploadMessages.TimedOut);
        }

        [Test]
        public void WhenUploadThrows_TaskCanceledException_DueToUserCancelling_Then_ErrorIsShownAnUploadStopped()
        {
            A.CallTo(() => _helpers.IsCancellationRequested(A<CancellationToken>._)).Returns(true);
            ExceptionErrorTest(new TaskCanceledException(), UploadMessages.Cancelled, true);
        }

        #endregion

        #region UpLoad - maxlag
        [Test]
        public void When_MaxlagResponse_Then_UpoloadIsRetried()
        {
            AlllFilesPermitted();
            AddSingleUploadFile();
            A.CallTo(() => _uploadResponse.Result)
                .Returns(ResponseCodes.MaxlagThrottle).Once()
                .Then.Returns(ResponseCodes.Success);

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, A<bool>._, A<bool>._))
                .MustHaveHappened(2, Times.Exactly);
        }

        [Test]
        public void When_MaxlagResponse_Then_UpoloadIsRetriedAMaximumOfThreeTimes()
        {
            AlllFilesPermitted();
            AddSingleUploadFile();
            A.CallTo(() => _uploadResponse.Result)
                .Returns(ResponseCodes.MaxlagThrottle);

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, A<bool>._, A<bool>._))
                .MustHaveHappened(4, Times.Exactly);
        }

        [Test]
        public void When_MaxlagExceedsMaximumRetries_Then_ServerIsBusyError()
        {
            AlllFilesPermitted();
            AddSingleUploadFile();
            A.CallTo(() => _uploadResponse.Result)
                .Returns(ResponseCodes.MaxlagThrottle);

            _model.UploadCommand.Execute(null);

            Assert.That(_model.UploadFiles.Count, Is.EqualTo(1));
            Assert.That(_model.UploadFiles[0].Message, Is.EqualTo(UploadMessages.ServerBusy));
        }

        [Test]
        public void When_MaxlagResponse_Then_SuggestedBackoffIsHonoured()
        {
            AlllFilesPermitted();
            AddSingleUploadFile();

            A.CallTo(() => _uploadResponse.Result)
                .Returns(ResponseCodes.MaxlagThrottle);
            const int backoffSeconds = 666;
            A.CallTo(() => _uploadResponse.RetryDelay)
                .Returns(backoffSeconds);

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _helpers.Wait(backoffSeconds * 1000, A<CancellationToken>._))
                .MustHaveHappened();
        }

        #endregion

        #region Sign Out
        [Test]
        public void When_SignOutIsExecuted_Then_UserIsLoggedOff()
        {
            _model.SignOutCommand.Execute(null);

            A.CallTo(() => _fileUploader.LogOff())
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_SignOutIsExecuted_Then_LoginPageIsNavigatedTo()
        {
            _model.SignOutCommand.Execute(null);

            A.CallTo(() => _navigationService.NavigateToLoginPage())
                .MustHaveHappened(1, Times.Exactly);
        }

        #endregion

        #region Add Category and Add Template

        [Test]
        public void When_PickCategoryIsExecuted_Then_SearchPageIsNavigatedTo()
        {
            _model.PickCategoryCommand.Execute(null);

            A.CallTo(() => _navigationService.NavigateToSearchPage())
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_PickCategoryIsExecuted_Then_CurrentSearchIsCategory()
        {
            _model.PickCategoryCommand.Execute(null);

            Assert.That(_model.CurrentSearch, Is.EqualTo(_categorySearch));
        }

        [Test]
        public void When_PickTemplateIsExecuted_Then_SearchPageIsNavigatedTo()
        {
            _model.PickTemplateCommand.Execute(null);

            A.CallTo(() => _navigationService.NavigateToSearchPage())
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_PickTemplateIsExecuted_Then_CurrentSearchIsTemplate()
        {
            _model.PickTemplateCommand.Execute(null);

            Assert.That(_model.CurrentSearch, Is.EqualTo(_templateSearch));
        }

        [Test]
        public void When_CategorySearchInProgress_Then_AnotherSearchCannotBeDone()
        {
            _model.SearchFetchInProgress = true;
            _model.CurrentSearch = _categorySearch;
            
            _model.StartSearchCommand.Execute(null);
            _model.NextSearchCommand.Execute(null);
            _model.PreviousSearchCommand.Execute(null);
            
            Assert.That(_model.SearchFetchInProgress, Is.True);
            A.CallTo(() => _categorySearch.Start(A<string>._))
                .MustNotHaveHappened();
            A.CallTo(() => _categorySearch.Next())
                .MustNotHaveHappened();
            A.CallTo(() => _categorySearch.Previous())
                .MustNotHaveHappened();
        }

        [Test]
        public void When_TemplateSearchInProgress_Then_AnotherSearchCannotBeDone()
        {
            _model.SearchFetchInProgress = true;
            _model.CurrentSearch = _templateSearch;
            
            _model.StartSearchCommand.Execute(null);
            _model.NextSearchCommand.Execute(null);
            _model.PreviousSearchCommand.Execute(null);

            Assert.That(_model.SearchFetchInProgress, Is.True);
            A.CallTo(() => _templateSearch.Start(A<string>._))
                .MustNotHaveHappened();
            A.CallTo(() => _templateSearch.Next())
                .MustNotHaveHappened();
            A.CallTo(() => _templateSearch.Previous())
                .MustNotHaveHappened();
        }

        [Test]
        public void When_StartSearchIsExecuted_Then_Start_SearchIsDoneOnCurrentSearch()
        {
            _model.SearchFetchInProgress = false;
            _model.CurrentSearch = _templateSearch;
            
            _model.StartSearchCommand.Execute(null);

            A.CallTo(() => _model.CurrentSearch.Start(A<string>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_NextSearchIsExecuted_Then_NextSearchIsDoneOnCurrentSearch()
        {
            A.CallTo(() => _templateSearch.HasNext).Returns(true);
            _model.SearchFetchInProgress = false;
            _model.CurrentSearch = _templateSearch;
            
            _model.NextSearchCommand.Execute(null);

            A.CallTo(() => _model.CurrentSearch.Next())
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_NextSearchIsExecutedWithNoMoreResults_Then_NoSeachIsMade()
        {
            A.CallTo(() => _templateSearch.HasNext).Returns(false);
            _model.SearchFetchInProgress = false;
            _model.CurrentSearch = _templateSearch;
            
            _model.NextSearchCommand.Execute(null);

            A.CallTo(() => _model.CurrentSearch.Next())
                .MustNotHaveHappened();
        }

        [Test]
        public void When_PreviousSearchIsExecuted_Then_PreviousSearchIsDoneOnCurrentSearch()
        {
            A.CallTo(() => _templateSearch.HasPrevious).Returns(true);
            _model.SearchFetchInProgress = false;
            _model.CurrentSearch = _templateSearch;
            
            _model.PreviousSearchCommand.Execute(null);

            A.CallTo(() => _model.CurrentSearch.Previous())
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_PreviousSearchIsExecutedAndNoPrevious_Then_PNoSeachIsMade()
        {
            A.CallTo(() => _templateSearch.HasPrevious).Returns(false);
            _model.SearchFetchInProgress = false;
            _model.CurrentSearch = _templateSearch;
            
            _model.PreviousSearchCommand.Execute(null);

            A.CallTo(() => _model.CurrentSearch.Previous())
                .MustNotHaveHappened();
        }

        [Test]
        public void When_StartSearchIsExecuted_Then_SearchInProgressIsSetToTrue()
        {
            _model.SearchFetchInProgress = false;
            _model.CurrentSearch = _templateSearch;
            var searchInProgressWasSetToTrue = false;

            _model.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_model.SearchFetchInProgress))
                    searchInProgressWasSetToTrue = true;
            };

            _model.StartSearchCommand.Execute(null);

            Assert.That(_model.SearchFetchInProgress, Is.False);
            Assert.That(searchInProgressWasSetToTrue, Is.True);
        }

        [Test]
        public void When_NextSearchIsExecuted_Then_SearchInProgressIsSetToTrue()
        {
            A.CallTo(() => _templateSearch.HasNext).Returns(true);
            _model.SearchFetchInProgress = false;
            _model.CurrentSearch = _templateSearch;
            var searchInProgressWasSetToTrue = false;

            _model.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_model.SearchFetchInProgress))
                    searchInProgressWasSetToTrue = true;
            };

            _model.NextSearchCommand.Execute(null);

            Assert.That(_model.SearchFetchInProgress, Is.False);
            Assert.That(searchInProgressWasSetToTrue, Is.True);
        }

        [Test]
        public void When_PreviousSearchIsExecuted_Then_SearchInProgressIsSetToTrue()
        {
            A.CallTo(() => _templateSearch.HasPrevious).Returns(true);
            _model.SearchFetchInProgress = false;
            _model.CurrentSearch = _templateSearch;
            var searchInProgressWasSetToTrue = false;

            _model.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_model.SearchFetchInProgress))
                    searchInProgressWasSetToTrue = true;
            };

            _model.PreviousSearchCommand.Execute(null);

            Assert.That(_model.SearchFetchInProgress, Is.False);
            Assert.That(searchInProgressWasSetToTrue, Is.True);
        }

        [Test]
        public void When_ContentIsEmptyAndSearchItemAdded_Then_NoNewlinwIsAdded()
        {
            _model.PageContent = "";
            _model.CurrentSearch = _categorySearch;

            _model.AddSearchItemCommand.Execute("foo");

            Assert.That(_model.PageContent, Does.Not.Contain("\n"));
        }

        [Test]
        public void When_ContentEndsWithNewlineAndSearchItemAdded_Then_NoNewlinwIsAdded()
        {
            _model.PageContent = "bar\n";
            _model.CurrentSearch = _categorySearch;

            _model.AddSearchItemCommand.Execute("foo");

            Assert.That(_model.PageContent, Does.Not.Contain("\n\n"));
        }

        [Test]
        public void When_ContentDoesNotEndsWithNewlineAndSearchItemAdded_Then_NewlinwIsAdded()
        {
            _model.PageContent = "bar";
            _model.CurrentSearch = _categorySearch;

            _model.AddSearchItemCommand.Execute("foo");

            Assert.That(_model.PageContent, Does.Contain("\n"));
        }

        [Test]
        public void When_SearchItemIsAdded_Then_FullTextIsUsed()
        {
            _model.PageContent = "";
            _model.CurrentSearch = _categorySearch;
            const string fullString = "bar";
            A.CallTo(() => _categorySearch.FullItemString(A<string>._))
                .Returns(fullString);

            _model.AddSearchItemCommand.Execute("foo");

            Assert.That(_model.PageContent, Is.EqualTo(fullString));
        }

        #endregion
    }
}
