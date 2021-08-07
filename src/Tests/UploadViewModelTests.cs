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
using WikiUpload.Properties;

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
        private IFileFinder _fileFinder;
        private IReadOnlyResponseErrors _responseErrors;
        private IReadOnlyResponseWarnings _responseWarnings;
        private IUploadListSerializer _uploadListSerializer;
        private ISiteInfo _siteInfo;
        private IReadOnlyPermittedFiles _permittedFiles;
        private UploadViewModel _model;

        #region Setup
        [SetUp]
        public void Setup()
        {
            _siteInfo = A.Fake<ISiteInfo>();
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
            _fileFinder = A.Fake<IFileFinder>();
            _responseErrors = A.Fake<IReadOnlyResponseErrors>();
            _responseWarnings = A.Fake<IReadOnlyResponseWarnings>();

            A.CallTo(() => _fileUploader.PermittedFiles)
                .Returns(_permittedFiles);
            A.CallTo(() => _fileUploader.SiteInfo)
                .Returns(_siteInfo);

            A.CallTo(() => _wikiSearchFactory.CreateCategorySearch(A<IFileUploader>._))
                .Returns(_categorySearch);
            A.CallTo(() => _wikiSearchFactory.CreateTemplateSearch(A<IFileUploader>._))
                .Returns(_templateSearch);

            A.CallTo(() => _uploadResponse.Errors).Returns(_responseErrors);
            A.CallTo(() => _uploadResponse.Warnings).Returns(_responseWarnings);

            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, A<string>._, A<string>._))
                .Returns(_uploadResponse);

            _model = new UploadViewModel(_fileUploader,
                _dialogs,
                _helpers,
                _uploadListSerializer,
                _navigationService,
                _wikiSearchFactory,
                _youtube,
                _fileFinder,
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

            A.CallTo(() => _dialogs.LoadContentDialog())
                .Returns(new PathDialogResponse
                {
                    Ok=true,
                    Path = thePath,
                });
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

            A.CallTo(() => _dialogs.LoadContentDialog())
                .Returns(new PathDialogResponse { Ok = false });
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

            A.CallTo(() => _dialogs.SaveContentDialog())
                .Returns(new PathDialogResponse
                {
                    Ok=true,
                    Path = thePath,
                });
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

            A.CallTo(() => _dialogs.SaveContentDialog())
                .Returns(new PathDialogResponse { Ok = false });

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
            A.CallTo(() => _helpers.ReadAllText(A<string>._))
                .Throws(new Exception());
            A.CallTo(() => _dialogs.LoadContentDialog())
                .Returns(new PathDialogResponse
                {
                    Ok = true,
                    Path = thePath,
                });

            _model.LoadContentCommand.Execute(null);

            A.CallTo(() => _dialogs.ErrorMessage("Unable to read content.", A<Exception>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_PageContentWriteError_Then_ErrorIsShown()
        {
            const string thePath = "foobar.txt";
            A.CallTo(() => _helpers.WriteAllText(A<string>._, A<string>._))
                .Throws(new Exception());
            A.CallTo(() => _dialogs.SaveContentDialog())
                .Returns(new PathDialogResponse
                {
                    Ok = true,
                    Path = thePath,
                });

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
            const string loadFilePath = "foobar.wul";
            const string filePath = "foobar.jpg";

            A.CallTo(() => _dialogs.LoadUploadListDialog(out path))
                .Returns(true)
                .AssignsOutAndRefParameters(loadFilePath);

            A.CallTo(() => _uploadListSerializer.Deserialize(loadFilePath)).
                Returns(new List<UploadFile>
                {
                    new UploadFile
                    {
                        FullPath = filePath,
                    }
                }) ;

            _model.LoadListCommand.Execute(null);

            Assert.That(_model.UploadFiles.Count, Is.EqualTo(1));
            Assert.That(_model.UploadFiles[0].FullPath, Is.EqualTo(filePath));
        }

        [Test]
        public void When_LoadUploadFilesIsExecutedAndCancelled_Then_UploadFilesIsUnchanged()
        {
            string path;

            A.CallTo(() => _dialogs.LoadUploadListDialog(out path))
                .Returns(false);

            _model.LoadListCommand.Execute(null);

            A.CallTo(() => _uploadListSerializer.Deserialize(A<string>._))
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

            A.CallTo(() => _uploadListSerializer.Serialize(thePath, _model.UploadFiles))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_SaveUploadFilesIsExecutedAndCancelled_Then_UploadFilesNotSaved()
        {
            string path;

            A.CallTo(() => _dialogs.SaveUploadListDialog(out path))
                .Returns(false);

            _model.SaveListCommand.Execute(null);

            A.CallTo(() => _uploadListSerializer.Serialize(A<string>._, _model.UploadFiles))
                .MustNotHaveHappened();
        }

        [Test]
        public void When_LoadUploadFilesReadError_Then_ErrorMessageIsShown()
        {
            string path;
            const string thePath = "foobar.wul";

            A.CallTo(() => _uploadListSerializer.Deserialize(A<string>._))
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

            A.CallTo(() => _uploadListSerializer.Serialize(A<string>._, A<UploadList>._))
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
        public void When_PlaylistIsDropped_Then_VideosInPlaylistAreAdded()
        {
            const string video1 = "one";
            const string video2 = "two";
            A.CallTo(() => _youtube.FetchPlasylistViedeoLinksAsync(A<string>._, A<int>._))
                .Returns(new string[] {
                    video1,
                    video2
                });
            var dropFiles = new string[] { "https://youtube.com?v=A&list=A" };
            SynchronizationContext.SetSynchronizationContext(new SynchonousSynchronizationContext());

            _model.OnFileDrop(dropFiles, false);

            Assert.That(_model.UploadFiles[0].FullPath, Is.EqualTo(video1));
            Assert.That(_model.UploadFiles[1].FullPath, Is.EqualTo(video2));
        }

        [Test]
        public void When_LongPlaylistIsDropped_Then_ErrorMessageIsShown()
        {
            A.CallTo(() => _youtube.FetchPlasylistViedeoLinksAsync(A<string>._, A<int>._))
                .ThrowsAsync(new TooManyVideosException());

            var dropFiles = new string[] { "https://youtube.com?v=A&list=A" };
            SynchronizationContext.SetSynchronizationContext(new SynchonousSynchronizationContext());

            _model.OnFileDrop(dropFiles, false);

            var expectedMessage = Resources.PlalistTooBig;
            A.CallTo(() => _dialogs.ErrorMessage(expectedMessage, A<string>._, A<bool>._))
                .MustHaveHappened(1, Times.Exactly);
        }


        [Test]
        public void When_PlaylistIsDroppedAndThereIsException_Then_ErrorMessageIsShown()
        {
            A.CallTo(() => _youtube.FetchPlasylistViedeoLinksAsync(A<string>._, A<int>._))
                .ThrowsAsync(new Exception("foobar"));

            var dropFiles = new string[] { "https://youtube.com?v=A&list=A" };
            SynchronizationContext.SetSynchronizationContext(new SynchonousSynchronizationContext());

            _model.OnFileDrop(dropFiles, false);

            var expectedMessage = Resources.YoutubeError;
            A.CallTo(() => _dialogs.ErrorMessage(expectedMessage))
                .MustHaveHappened(1, Times.Exactly);
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

        #region Add Folder

        [Test]
        public void When_AddFolderIsExecuted_Then_AddFolderDialogIsShown()
        {
            _model.AddFolderCommand.Execute(null);

            A.CallTo(() => _dialogs.AddFolderDialog()).MustHaveHappened();
        }

        [Test]
        public void When_AddFolderIsExecutedAndFolderSelected_Then_AddFolderOptionsDialogIsShown()
        {
            A.CallTo(() => _dialogs.AddFolderDialog())
                .Returns(new PathDialogResponse { Ok = true });

            _model.AddFolderCommand.Execute(null);

            A.CallTo(() => _dialogs.AddFolderOptionsDialog(A<string>._))
                .MustHaveHappened();
        }

        [Test]
        public void When_AddFolderIsExecutedAndNoFolderSelected_Then_AddFolderOptionsDialogIsNotShown()
        {
            A.CallTo(() => _dialogs.AddFolderDialog())
                .Returns(new PathDialogResponse { Ok = false });

            _model.AddFolderCommand.Execute(null);

            A.CallTo(() => _dialogs.AddFolderOptionsDialog(A<string>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void When_AddFolderIsExecuted_Then_MatchingFilesAreAdded()
        {
            A.CallTo(() => _dialogs.AddFolderDialog())
                .Returns(new PathDialogResponse { Ok = true });
            A.CallTo(() => _dialogs.AddFolderOptionsDialog(A<string>._))
                .Returns(new AddFolderOptionsDialogResponse { Ok = true });
            var files = new List<string> { "foo", "bar" };
            A.CallTo(() => _fileFinder.GetFiles(A<string>._, A<bool>._, A<IncludeFiles>._, A<string>._))
                .Returns(files);

            _model.AddFolderCommand.Execute(null);

            Assert.That(_model.UploadFiles.Select(x => x.FullPath), Is.EquivalentTo(files));
        }

        [Test]
        public void When_AddFolderIsExecutedAndCancelledAtOptions_ThenNoFilesAreAdded()
        {
            A.CallTo(() => _dialogs.AddFolderDialog())
                .Returns(new PathDialogResponse { Ok = true });
            A.CallTo(() => _dialogs.AddFolderOptionsDialog(A<string>._))
                .Returns(new AddFolderOptionsDialogResponse { Ok = false });
            var files = new List<string> { "foo", "bar" };
            A.CallTo(() => _fileFinder.GetFiles(A<string>._, A<bool>._, A<IncludeFiles>._, A<string>._))
                .Returns(files);

            _model.AddFolderCommand.Execute(null);

            Assert.That(_model.UploadFiles, Is.Empty);
        }

        [Test]
        public void When_AddFolderIsExecutedAndExceptionOccurs_Then_ErrorIsReported()
        {
            A.CallTo(() => _dialogs.AddFolderDialog())
                .Returns(new PathDialogResponse { Ok = true });
            A.CallTo(() => _dialogs.AddFolderOptionsDialog(A<string>._))
                .Returns(new AddFolderOptionsDialogResponse { Ok = true });

            var expectedException = new Exception("Error Message");

            A.CallTo(() => _fileFinder.GetFiles(A<string>._, A<bool>._, A<IncludeFiles>._, A<string>._))
                .Throws(expectedException);

            _model.AddFolderCommand.Execute(null);

            A.CallTo(() => _dialogs.ErrorMessage(Resources.UnableToAddFiles, expectedException))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_AddFolderIsExecutedAndExceptionOccurs_Then_NoLongerAddingFolders()
        {
            A.CallTo(() => _dialogs.AddFolderDialog())
                .Returns(new PathDialogResponse { Ok = true });
            A.CallTo(() => _dialogs.AddFolderOptionsDialog(A<string>._))
                .Returns(new AddFolderOptionsDialogResponse { Ok = true });

            var expectedException = new Exception("Error Message");

            A.CallTo(() => _fileFinder.GetFiles(A<string>._, A<bool>._, A<IncludeFiles>._, A<string>._))
                .Throws(expectedException);

            _model.AddFolderCommand.Execute(null);

            Assert.That(_model.AddingFiles, Is.False);
        }

        #endregion

        #region Process Launch

        [Test]
        public void When_LaunchSiteIsExecuted_Then_SiteHomePageIsLaunched()
        {
            const string baseUrl = "foobar";
            A.CallTo(() => _siteInfo.BaseUrl).Returns(baseUrl);

            _model.LaunchSiteCommand.Execute(null);

            A.CallTo(() => _helpers.LaunchProcess(baseUrl))
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
            for (var i = 0; i < 3; i++)
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

            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void When_UploadIsDone_Then_PageContentUsed()
        {
            AlllFilesPermitted();
            AddSingleUploadFile();
            A.CallTo(() => _uploadResponse.Result).Returns(ResponseCodes.Success);
            const string content = "foobar";
            _model.PageContent = content;

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _fileUploader.UpLoadAsync(
                A<string>.Ignored,
                A<CancellationToken>.Ignored,
                A<string>.Ignored,
                content)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_UploadIsDone_Then_SummaryContainsViaWikiUp()
        {
            AlllFilesPermitted();
            AddSingleUploadFile();
            A.CallTo(() => _uploadResponse.Result).Returns(ResponseCodes.Success);
            const string summary = "foobar";
            _model.UploadSummary = summary;

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _fileUploader.UpLoadAsync(
                A<string>.Ignored,
                A<CancellationToken>.Ignored,
                A<string>.That.Contains("via Wiki-Up"),
                A<string>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_MustBeLoggedInError_Then_MessageIsShownAndLoginNavigatedTo()
        {
            AlllFilesPermitted();
            AddSingleUploadFile();
            A.CallTo(() => _responseErrors.IsMutsBeLoggedInError).Returns(true);
            A.CallTo(() => _responseErrors.IsAny).Returns(true);
            A.CallTo(() => _dialogs.ErrorMessage(A<string>._, A<string>._, A<bool>._))
                .Returns(true);

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _dialogs.ErrorMessage(Resources.LoginExpiredText, Resources.LoginExpiredSubtext, true))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => _navigationService.NavigateToLoginPage())
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_MustBeLoggedInError_Then_UploadDoesNotContinue()
        {
            AlllFilesPermitted();
            AddThreeUploadFiles();
            A.CallTo(() => _responseErrors.IsMutsBeLoggedInError).Returns(true);
            A.CallTo(() => _responseErrors.IsAny).Returns(true);

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _fileUploader.UpLoadAsync(
                A<string>.Ignored,
                A<CancellationToken>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }


        [Test]
        public void When_UploadIsDoneToFandom_Then_SummaryIsSetWithLinkToDev()
        {
            AlllFilesPermitted();
            AddSingleUploadFile();
            A.CallTo(() => _uploadResponse.Result).Returns(ResponseCodes.Success);
            const string summary = "foobar";
            _model.UploadSummary = summary;

            A.CallTo(() => _siteInfo.ServerUrl)
                .Returns(".fandom.");

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _fileUploader.UpLoadAsync(
                A<string>.Ignored,
                A<CancellationToken>.Ignored,
                A<string>.That.Contains("[[w:c:dev:Wiki-Up|Wiki-Up]]"),
                A<string>.Ignored)).MustHaveHappened(1, Times.Exactly);
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

            A.CallTo(() => _responseErrors.IsAny).Returns(true);
            A.CallTo(() => _responseErrors.IsTokenError).Returns(false);
            const string errorText = "foobar";
            A.CallTo(() => _responseErrors.ToString()).Returns(errorText);

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
            A.CallTo(() => _responseWarnings.ToString()).Returns(warningText);

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

            A.CallTo(() => _fileUploader.UpLoadAsync(
                A<string>.Ignored,
                A<CancellationToken>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored)).MustNotHaveHappened();
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
            var wasSetToUnloading = false;
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
        public void When_SummaryHasVariablea_Then_SimmaryIsExpanded()
        {
            AlllFilesPermitted();
            AddSingleUploadFile();
            _model.UploadSummary = "<%filename>";

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _fileUploader.UpLoadAsync(
                A<string>.Ignored,
                A<CancellationToken>.Ignored,
                A<string>.That.StartsWith("Foobar"),
                A<string>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_ContentHasVariablea_Then_ContentIsExpanded()
        {
            AlllFilesPermitted();
            AddSingleUploadFile();
            _model.PageContent = "<%filename>";

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _fileUploader.UpLoadAsync(
                A<string>.Ignored,
                A<CancellationToken>.Ignored,
                A<string>.Ignored,
                A<string>.That.StartsWith("Foobar"))).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_ForceUploadsIsOn_Then_UploadIsDoneWithIgnoreWarnings()
        {
            AlllFilesPermitted();
            AddThreeUploadFiles();
            _model.ForceUpload = true;

            _model.UploadCommand.Execute(null);

            A.CallToSet(() => _fileUploader.IgnoreWarnings).To(true).MustHaveHappened();
            A.CallToSet(() => _fileUploader.IgnoreWarnings).To(false).MustNotHaveHappened();
        }

        [Test]
        public void When_ForceUploadsIsOff_Then_UploadIsDoneWithoutIgnoreWarnings()
        {
            AlllFilesPermitted();
            AddThreeUploadFiles();
            _model.ForceUpload = false;

            _model.UploadCommand.Execute(null);

            A.CallToSet(() => _fileUploader.IgnoreWarnings).To(false).MustHaveHappened();
            A.CallToSet(() => _fileUploader.IgnoreWarnings).To(true).MustNotHaveHappened();
        }

        [Test]
        public void When_IncludeInWatchlistIsOn_Then_UploadIsDoneWithIncludeInWatchlist()
        {
            AlllFilesPermitted();
            AddThreeUploadFiles();
            _model.IncludeInWatchlist = true;

            _model.UploadCommand.Execute(null);

            A.CallToSet(() => _fileUploader.IncludeInWatchList).To(true).MustHaveHappened();
            A.CallToSet(() => _fileUploader.IncludeInWatchList).To(false).MustNotHaveHappened();
        }

        [Test]
        public void When_IncludeInWatchlistIsOff_Then_UploadIsDoneWithoutIncludeInWatchlist()
        {
            AlllFilesPermitted();
            AddThreeUploadFiles();
            _model.IncludeInWatchlist = false;

            _model.UploadCommand.Execute(null);

            A.CallToSet(() => _fileUploader.IncludeInWatchList).To(false).MustHaveHappened();
            A.CallToSet(() => _fileUploader.IncludeInWatchList).To(true).MustNotHaveHappened();
        }

        [Test]
        public void When_UploadingAndFollowEnabled_Then_UploadingFileIsBroughtIntoView()
        {
            AlllFilesPermitted();
            var files = new List<UploadFile>
            {
                new UploadFile { FullPath = "foo.jpg" },
                new UploadFile { FullPath = "bar.jpg" },
                new UploadFile { FullPath = "baz.jpg" },
                new UploadFile { FullPath = "https://foobar/fklfk" },
            };
            _model.UploadFiles.AddRange(files);
            A.CallTo(() => _appSetttings.FollowUploadFile).Returns(true);
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
        public void When_UploadingAndFollowDisabled_Then_UploadingFileIsNotBroughtIntoView()
        {
            AlllFilesPermitted();
            var files = new List<UploadFile>
            {
                new UploadFile { FullPath = "foo.jpg" },
                new UploadFile { FullPath = "bar.jpg" },
                new UploadFile { FullPath = "baz.jpg" },
                new UploadFile { FullPath = "https://foobar/fklfk" },
            };
            _model.UploadFiles.AddRange(files);
            A.CallTo(() => _appSetttings.FollowUploadFile).Returns(false);
            var viewdFiles = new List<UploadFile>();
            _model.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_model.ViewedFile) && _model.ViewedFile != null)
                    viewdFiles.Add(_model.ViewedFile);
            };

            _model.UploadCommand.Execute(null);

            Assert.That(viewdFiles, Is.Empty);
        }


        [Test]
        public void When_InvalidTokenResponse_Then_OneAttemptIsMadeToRefreshToken()
        {
            AlllFilesPermitted();
            AddThreeUploadFiles();
            A.CallTo(() => _responseErrors.IsAny).Returns(true);
            A.CallTo(() => _responseErrors.IsTokenError).Returns(true);

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
            A.CallTo(() => _responseErrors.IsAny)
                .Returns(true).Once().Then.Returns(false);
            A.CallTo(() => _responseErrors.IsTokenError)
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

        #region Upload - Videos

        [Test]
        public void When_UploadFileIsVideo_Then_VideoIsUploaded()
        {
            _model.UploadFiles.Clear();
            _model.UploadFiles.Add(new UploadFile
            {
                FullPath = "https://foobar.com"
            });
            _model.UploadFiles.Add(new UploadFile
            {
                FullPath = "https://youtube.com"
            });

            _model.UploadCommand.Execute(null);

            A.CallTo(() => _fileUploader.UpLoadVideoAsync(A<string>._, A<CancellationToken>._))
                .MustHaveHappened(2, Times.Exactly);
            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        #endregion

        #region Upload - Exception Errors

        private void ExceptionErrorTest(Exception errorException, string expedtedMessage, bool stopsUpload = false)
        {
            AlllFilesPermitted();
            AddThreeUploadFiles();
            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, A<string>._,  A<string>._))
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

            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, A<string>._, A<string>._))
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

            A.CallTo(() => _fileUploader.UpLoadAsync(A<string>._, A<CancellationToken>._, A<string>._, A<string>._))
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
