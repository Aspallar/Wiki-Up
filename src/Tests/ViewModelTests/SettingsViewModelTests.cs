using FakeItEasy;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using WikiUpload;
using WikiUpload.Properties;

namespace Tests.ViewModelTests
{
    [TestFixture]
    public class SettingsViewModelTests
    {
        private IAppSettings _appSettings;
        private INavigatorService _navigatorService;
        private IUpdateCheck _updateCheck;
        private IWindowManager _windowManager;
        private IHelpers _helpers;
        private SettingsViewModel _model;

        [SetUp]
        public void Setup()
        {
            _appSettings = A.Fake<IAppSettings>();
            _navigatorService = A.Fake<INavigatorService>();
            _updateCheck = A.Fake<IUpdateCheck>();
            _windowManager = A.Fake<IWindowManager>();
            _helpers = A.Fake<IHelpers>();
            _model = new SettingsViewModel(_appSettings, _navigatorService, _updateCheck, _windowManager, _helpers);
        }

        [Test]
        public void When_SaveSettingsIsExecuted_Then_SettingsAreSaved()
        {
            _model.SelectedLanguage = _model.Languages[0];
            _model.Delay = 666;
            _model.ImageFileExtensions = new FileExensionsCollection("foo");
            _model.CheckForUpdates = true;
            _model.MainWindowPlacementEnabled = true;
            _model.UploadedWindowPlacementEnabled = true;

            _model.SaveSettingsCommand.Execute(null);

            A.CallToSet(() => _appSettings.Language).To(() => _model.Languages[0].Code)
                .MustHaveHappened(1, Times.Exactly);
            A.CallToSet(() => _appSettings.UploadDelay).To(() => _model.Delay)
                .MustHaveHappened(1, Times.Exactly);
            A.CallToSet(() => _appSettings.ImageExtensions).To(() => "foo;")
                .MustHaveHappened(1, Times.Exactly);
            A.CallToSet(() => _appSettings.CheckForUpdates).To(() => _model.CheckForUpdates)
                .MustHaveHappened(1, Times.Exactly);
            A.CallToSet(()=>_appSettings.FollowUploadFile).To(() => _model.FollowUploadFile)
                .MustHaveHappened(1, Times.Exactly);
            A.CallToSet(()=>_appSettings.UploadedWindowPlacementEnabled).To(() => _model.UploadedWindowPlacementEnabled)
                .MustHaveHappened(1, Times.Exactly);
            A.CallToSet(()=>_appSettings.MainWindowPlacementEnabled).To(() => _model.MainWindowPlacementEnabled)
                .MustHaveHappened(1, Times.Exactly);

            A.CallTo(()=>_appSettings.Save())
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_CancelIsExecuted_Then_SettingsAreNotSaved()
        {
            _model.SelectedLanguage = _model.Languages[0];
            _model.Delay = 666;
            _model.ImageFileExtensions = new FileExensionsCollection("fooo");
            _model.CheckForUpdates = true;
            _model.MainWindowPlacementEnabled = true;
            _model.UploadedWindowPlacementEnabled = true;

            _model.CancelSettingsCommand.Execute(null);

            A.CallToSet(() => _appSettings.Language).To(() => A<string>._)
                .MustNotHaveHappened();
            A.CallToSet(() => _appSettings.UploadDelay).To(() => A<int>._)
                .MustNotHaveHappened();
            A.CallToSet(() => _appSettings.ImageExtensions).To(() => A<string>._)
                .MustNotHaveHappened();
            A.CallToSet(() => _appSettings.CheckForUpdates).To(() => A<bool>._)
                .MustNotHaveHappened();
            A.CallToSet(() => _appSettings.FollowUploadFile).To(() => _model.FollowUploadFile)
                .MustNotHaveHappened();
            A.CallToSet(() => _appSettings.UploadedWindowPlacementEnabled).To(() => _model.UploadedWindowPlacementEnabled)
                .MustNotHaveHappened();
            A.CallToSet(() => _appSettings.MainWindowPlacementEnabled).To(() => _model.MainWindowPlacementEnabled)
                .MustNotHaveHappened();
            A.CallTo(() => _appSettings.Save())
                .MustNotHaveHappened();
        }

        [Test]
        public void When_CancelIsExecuted_Then_SettingsPageIsClosed()
        {
            _model.CancelSettingsCommand.Execute(null);

            A.CallTo(() => _navigatorService.LeaveSettingsPage())
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_SaveSettingsIsExecuted_Then_SettingsPageIsClosed()
        {
            _model.SaveSettingsCommand.Execute(null);

            A.CallTo(() => _navigatorService.LeaveSettingsPage())
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_ToggleAddImageExtensionPopupCommandIsExecutedWithPopupClosed_Then_AddIsStarted()
        {
            _model.IsAddingImageExtension = false;

            _model.ToggleAddImageExtensionPopupCommand.Execute(null);

            Assert.That(_model.IsAddingImageExtension, Is.True);
        }

        [Test]
        public void When_ToggleAddImageExtensionPopupCommandIsExecutedWithPopupOpen_Then_PopupIsClosed()
        {
            _model.IsAddingImageExtension = true;

            _model.ToggleAddImageExtensionPopupCommand.Execute(null);

            Assert.That(_model.IsAddingImageExtension, Is.False);
        }

        [Test]
        public void When_NewExtensionTextIsInvalid_Then_ArgumentExceptionIsThrown()
        {
            Assert.Throws<ArgumentException>(() => _model.NewExtensionText = "<");
        }

        [Test]
        public void When_NewExtensionTextIsSetToInvalidValue_Then_ValueIsSet()
        {
            const string invalidExtension = ">>>";
            try
            {
                _model.NewExtensionText = invalidExtension;
            }
            catch (ArgumentException) { }

            Assert.That(_model.NewExtensionText, Is.EqualTo(invalidExtension));
        }

        [Test]
        public void When_ToggleWindowPlacementPopupCommandExecutedWithPopupClosed_Then_PopupIsOpened()
        {
            _model.IsWindowPlacementPopupOpen = false;

            _model.ToggleWindowPlacementPopupCommand.Execute(null);

            Assert.That(_model.IsWindowPlacementPopupOpen, Is.True);
        }

        [Test]
        public void When_ToggleWindowPlacementPopupCommandIsExecutedWithPopupOpen_Then_PopupIsClosed()
        {
            _model.IsWindowPlacementPopupOpen = true;

            _model.ToggleWindowPlacementPopupCommand.Execute(null);

            Assert.That(_model.IsWindowPlacementPopupOpen, Is.False);
        }

        [Test]
        public void When_ExtensionIsInvalid_Then_ExtensaionIsNotAddedd()
        {
            _model.ImageFileExtensions = new FileExensionsCollection();

            _model.AddImageEtensionCommand.Execute("fo;o");
            _model.AddImageEtensionCommand.Execute("bar;");
            _model.AddImageEtensionCommand.Execute("foo>bar");

            Assert.That(_model.ImageFileExtensions, Is.Empty);
        }

        [Test]
        public void When_CheckForUpdatesNowIsExecuted_Then_UpdatesAreChecked()
        {
            _model.CheckForUpdatesNowCommand.Execute(null);

            A.CallTo(() => _updateCheck.CheckForUpdates(A<string>._, A<int>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_CheckForUpdatesNowIsExecuted_Then_CorrectUserAgentIsUsed()
        {
            const string userAgent = "Foobar";
            A.CallTo(()=> _helpers.UserAgent).Returns(userAgent);

            _model.CheckForUpdatesNowCommand.Execute(null);

            A.CallTo(() => _updateCheck.CheckForUpdates(userAgent, A<int>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_UpdateCheckAndUpToDate_Then_UpToDateMessageIsShown()
        {
            var checkForUpdatesResponse = new UpdateCheckResponse
            {
                IsNewerVersion = false,
            };
            A.CallTo(() => _updateCheck.CheckForUpdates(A<string>._, A<int>._))
                .Returns(Task.FromResult(checkForUpdatesResponse));
            _model.CheckUpdateMessage = "";

            _model.CheckForUpdatesNowCommand.Execute(null);

            Assert.That(_model.CheckUpdateMessage, Is.EqualTo(WikiUpload.Properties.Resources.UpToDateText));
            Assert.That(_model.IsCheckForUpdateMessage, Is.True);
        }

        [Test]
        public void When_UpdateCheckAndNewerVersion_Then_NoMessageIsShown()
        {
            var checkForUpdatesResponse = new UpdateCheckResponse
            {
                IsNewerVersion = true,
            };
            A.CallTo(() => _updateCheck.CheckForUpdates(A<string>._, A<int>._))
                .Returns(Task.FromResult(checkForUpdatesResponse));
            _model.CheckUpdateMessage = "";

            _model.CheckForUpdatesNowCommand.Execute(null);

            Assert.That(_model.CheckUpdateMessage, Is.Empty);
            Assert.That(_model.IsCheckForUpdateMessage, Is.False);
        }

        [Test]
        public void When_UpdateCheckAndBewerVersion_Then_NewVersionWindowIsShown()
        {
            var checkForUpdatesResponse = new UpdateCheckResponse
            {
                IsNewerVersion = true,
            };
            A.CallTo(() => _updateCheck.CheckForUpdates(A<string>._, A<int>._))
                .Returns(Task.FromResult(checkForUpdatesResponse));
            _model.CheckUpdateMessage = "";

            _model.CheckForUpdatesNowCommand.Execute(null);

            A.CallTo(() => _windowManager.ShowNewVersionWindow(checkForUpdatesResponse, false))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_RestoreDefaultsIsExecuted_Then_DefaultsAreRestored()
        {
            _model.RestoreDefaultsCommand.Execute(null);

            A.CallTo(() => _appSettings.RestoreConfigurationDefaults())
                .MustHaveHappened(1, Times.Exactly);
        }
    }
}
