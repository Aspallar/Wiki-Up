using FakeItEasy;
using NUnit.Framework;
using System.Collections.Generic;
using WikiUpload;
using WikiUpload.Properties;

namespace Tests
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

            _model.SaveSettingsCommand.Execute(null);

            A.CallToSet(() => _appSettings.Language).To(() => _model.Languages[0].Code)
                .MustHaveHappened(1, Times.Exactly);
            A.CallToSet(() => _appSettings.UploadDelay).To(() => _model.Delay)
                .MustHaveHappened(1, Times.Exactly);
            A.CallToSet(() => _appSettings.ImageExtensions).To(() => "foo;")
                .MustHaveHappened(1, Times.Exactly);
            A.CallToSet(() => _appSettings.CheckForUpdates).To(() => _model.CheckForUpdates)
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

            _model.CancelSettingsCommand.Execute(null);

            A.CallToSet(() => _appSettings.Language).To(() => A<string>._)
                .MustNotHaveHappened();
            A.CallToSet(() => _appSettings.UploadDelay).To(() => A<int>._)
                .MustNotHaveHappened();
            A.CallToSet(() => _appSettings.ImageExtensions).To(() => A<string>._)
                .MustNotHaveHappened();
            A.CallToSet(() => _appSettings.CheckForUpdates).To(() => A<bool>._)
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
        public void When_OpenAddImageExtensionIsExecuted_Then_AddIsStarted()
        {
            _model.IsAddingImageExtension = false;

            _model.OpenAddImageExtensionCommand.Execute(null);

            Assert.That(_model.IsAddingImageExtension, Is.True);
        }

        [Test]
        public void When_ExtensionIsAdded_Then_SemiColonsAreStripped()
        {
            List<string> expected = new List<string> { "foo", "bar", "foobar" };
            _model.ImageFileExtensions = new FileExensionsCollection();

            _model.AddImageEtensionCommand.Execute("fo;o");
            _model.AddImageEtensionCommand.Execute("bar;");
            _model.AddImageEtensionCommand.Execute(";foobar");

            Assert.That(_model.ImageFileExtensions, Is.EquivalentTo(expected));
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
            var checkForUpdatesEventArgs = new CheckForUpdatesEventArgs
            {
                IsNewerVersion = false,
            };
            _model.CheckUpdateMessage = "";
            _updateCheck.CheckForUpdateCompleted += Raise.With(checkForUpdatesEventArgs);

            Assert.That(_model.CheckUpdateMessage, Is.EqualTo(WikiUpload.Properties.Resources.UpToDateText));
            Assert.That(_model.IsCheckForUpdateMessage, Is.True);
        }

        [Test]
        public void When_UpdateCheckAndBewerVersion_Then_NoMessageIsShown()
        {
            var checkForUpdatesEventArgs = new CheckForUpdatesEventArgs
            {
                IsNewerVersion = true,
            };
            _model.CheckUpdateMessage = "";
            _updateCheck.CheckForUpdateCompleted += Raise.With(checkForUpdatesEventArgs);

            Assert.That(_model.CheckUpdateMessage, Is.Empty);
            Assert.That(_model.IsCheckForUpdateMessage, Is.False);
        }

        [Test]
        public void When_UpdateCheckAndBewerVersion_Then_NewVersionWindowIsShown()
        {
            var checkForUpdatesEventArgs = new CheckForUpdatesEventArgs
            {
                IsNewerVersion = true,
            };
            _model.CheckUpdateMessage = "";
            _updateCheck.CheckForUpdateCompleted += Raise.With(checkForUpdatesEventArgs);

            A.CallTo(() => _windowManager.ShowNewVersionWindow(checkForUpdatesEventArgs))
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
