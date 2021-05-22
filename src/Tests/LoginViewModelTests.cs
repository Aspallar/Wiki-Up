using FakeItEasy;
using NUnit.Framework;
using System;
using System.Security;
using System.Threading.Tasks;
using WikiUpload;

namespace Tests
{
    [TestFixture]
    public class LoginViewModelTests
    {
        private INavigatorService _navigatorService;
        private IDialogManager _dialogs;
        private IPasswordManager _passwordManager;
        private WikiUpload.Properties.IAppSettings _appSetttings;
        private IHavePassword _password;
        private IFileUploader _fileUploader;
        private IHelpers _helpers;
        private LoginViewModel _model;

        [SetUp]
        public void Setup()
        {
            _navigatorService = A.Fake<INavigatorService>();
            _dialogs = A.Fake<IDialogManager>();
            _passwordManager = A.Fake<IPasswordManager>();
            _appSetttings = A.Fake<WikiUpload.Properties.IAppSettings>();
            _password = A.Fake<IHavePassword>();
            _fileUploader = A.Fake<IFileUploader>();
            _helpers = A.Fake<IHelpers>();

            _model = new LoginViewModel(_fileUploader,
                _navigatorService,
                _dialogs,
                _passwordManager,
                _helpers,
                _appSetttings);
        }

        [Test]
        public void When_aPoliticianIsSpeaking_Then_TheyAreLyinmg()
        {
            Assert.Pass();
        }

        [Test]
        public void When_NoWikiUrl_Then_LoginNotAttemptedAndErrorShown()
        {
            _model.Username = "Foo";

            _model.LoginCommand.Execute(_password);

            A.CallTo(() => _fileUploader.LoginAsync(A<string>._, A<string>._, A<SecureString>._, A<bool>._))
                .MustNotHaveHappened();
            Assert.That(_model.IsLoginError, Is.True);
            Assert.That(_model.LoginErrorMessage.Length, Is.GreaterThan(0));
        }

        [Test]
        public void When_InvalidikiUrl_Then_LoginNotAttemptedAbdErrorShown()
        {
            _model.WikiUrl = ".......";
            _model.Username = "Foo";

            _model.LoginCommand.Execute(_password);

            A.CallTo(() => _fileUploader.LoginAsync(A<string>._, A<string>._, A<SecureString>._, A<bool>._))
                .MustNotHaveHappened();
            Assert.That(_model.IsLoginError, Is.True);
            Assert.That(_model.LoginErrorMessage.Length, Is.GreaterThan(0));
        }

        [Test]
        public void When_NoUsername_Then_LoginNotAttemptedAndErrorShown()
        {
            _model.WikiUrl = "Foo";

            _model.LoginCommand.Execute(_password);

            A.CallTo(() => _fileUploader.LoginAsync(A<string>._, A<string>._, A<SecureString>._, A<bool>._))
                .MustNotHaveHappened();
            Assert.That(_model.IsLoginError, Is.True);
            Assert.That(_model.LoginErrorMessage.Length, Is.GreaterThan(0));
        }

        [Test]
        public void When_AttempedLoginFails_Then_ErrorMessageIsShown()
        {
            _model.WikiUrl = "Bar";
            _model.Username = "Foo";
            A.CallTo(() =>
                _fileUploader.LoginAsync(A<string>._, A<string>._, A<SecureString>._, A<bool>._))
                    .Returns(Task.FromResult(false));

            _model.LoginCommand.Execute(_password);

            Assert.That(_model.IsLoginError, Is.True);
            Assert.That(_model.LoginErrorMessage.Length, Is.GreaterThan(0));
        }

        [Test]
        public void When_AttempedLoginFailsWithException_Then_ErrorMessageIsShown()
        {
            _model.WikiUrl = "Bar";
            _model.Username = "Foo";
            const string errorMessage = "Foobar";
            A.CallTo(() =>
                _fileUploader.LoginAsync(A<string>._, A<string>._, A<SecureString>._, A<bool>._))
                    .Throws(new LoginException(errorMessage));

            _model.LoginCommand.Execute(_password);

            Assert.That(_model.IsLoginError, Is.True);
            Assert.That(_model.LoginErrorMessage, Is.EqualTo(errorMessage));
        }

        [Test]
        public void When_AttempedLoginFailsWithInnerException_Then_ErrorMessageIsShown()
        {
            _model.WikiUrl = "Bar";
            _model.Username = "Foo";
            const string errorMessage = "Foobar";
            A.CallTo(() =>
                _fileUploader.LoginAsync(A<string>._, A<string>._, A<SecureString>._, A<bool>._))
                    .Throws(new LoginException("FooBaz", new Exception(errorMessage)));

            _model.LoginCommand.Execute(_password);

            Assert.That(_model.IsLoginError, Is.True);
            Assert.That(_model.LoginErrorMessage, Is.EqualTo(errorMessage));
        }

        [Test]
        public void When_SavedPasswordHasPassword_Then_SavedPasswordIsUsedForLogin()
        {
            _model.WikiUrl = "Bar";
            _model.Username = "Foo";
            _model.SavedPassword.AppendChar('a');

            _model.LoginCommand.Execute(_password);

            A.CallTo(() => _fileUploader.LoginAsync(A<string>._, A<string>._, _model.SavedPassword, A<bool>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_SavedPasswordHasNoPassword_Then_FormPasswordIsUsedForLogin()
        {
            var password = new SecureString();
            A.CallTo(() => _password.SecurePassword).Returns(password);
            _model.WikiUrl = "Bar";
            _model.Username = "Foo";

            _model.LoginCommand.Execute(_password);

            A.CallTo(() => _fileUploader.LoginAsync(A<string>._, A<string>._, password, A<bool>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_LoggedInSuccessfully_Then_NavigatesToUploadPage()
        {
            _model.WikiUrl = "Bar";
            _model.Username = "Foo";

            A.CallTo(() =>
                _fileUploader.LoginAsync(A<string>._, A<string>._, A<SecureString>._, A<bool>._))
                    .Returns(Task.FromResult(true));

            _model.LoginCommand.Execute(_password);

            A.CallTo(() => _navigatorService.NavigateToUploadPage()).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_LoggedInSuccessfully_Then_SettingsAreSaved()
        {
            const string wiki = "Bar";
            const string user = "Foo";
            const RememberPasswordOptions rememberPassword = RememberPasswordOptions.RememberForSite;

            _model.WikiUrl = wiki;
            _model.Username = user;
            _model.RememberPassword = rememberPassword;

            A.CallTo(() =>
                _fileUploader.LoginAsync(A<string>._, A<string>._, A<SecureString>._, A<bool>._))
                    .Returns(Task.FromResult(true));

            _model.LoginCommand.Execute(_password);

            Assert.That(_appSetttings.RememberPassword, Is.EqualTo(rememberPassword));
            Assert.That(_appSetttings.WikiUrl, Is.EqualTo(wiki));
            Assert.That(_appSetttings.Username, Is.EqualTo(user));

            A.CallTo(() => _appSetttings.AddMostRecentlyUsedSite(wiki)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => _appSetttings.Save()).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_LogginFails_Then_SettingsAreNotSaved()
        {
            _model.WikiUrl = "Foo";
            _model.Username = "Bar";

            A.CallTo(() =>
                _fileUploader.LoginAsync(A<string>._, A<string>._, A<SecureString>._, A<bool>._))
                    .Returns(Task.FromResult(false));

            _model.LoginCommand.Execute(_password);

            A.CallTo(() => _appSetttings.Save()).MustNotHaveHappened();
        }

        [Test]
        public void Whaen_LoggedInSuccessfullyAndRememberPasswordIsSite_Then_PasswordIsSaved()
        {
            _model.WikiUrl = "Foo";
            _model.Username = "Bar";
            _model.RememberPassword = RememberPasswordOptions.RememberForSite;

            A.CallTo(() =>
                _fileUploader.LoginAsync(A<string>._, A<string>._, A<SecureString>._, A<bool>._))
                    .Returns(Task.FromResult(true));

            _model.LoginCommand.Execute(_password);

            A.CallTo(() => _passwordManager.SavePassword(A<string>._, A<string>._, A<SecureString>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void Whaen_LoggedInSuccessfullyAndRememberPasswordIsDomain_Then_DomainPasswordIsSaved()
        {
            _model.WikiUrl = "Foo";
            _model.Username = "Bar";
            _model.RememberPassword = RememberPasswordOptions.RememberForDomain;

            A.CallTo(() =>
                _fileUploader.LoginAsync(A<string>._, A<string>._, A<SecureString>._, A<bool>._))
                    .Returns(Task.FromResult(true));

            _model.LoginCommand.Execute(_password);

            A.CallTo(() => _passwordManager.SaveDomainPassword(A<string>._, A<string>._, A<SecureString>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void Whaen_LoggedInSuccessfullyAndRememberPasswordIsOff_Then_PasswordIsRemoved()
        {
            _model.WikiUrl = "Foo";
            _model.Username = "Bar";
            _model.RememberPassword = RememberPasswordOptions.DoNotRemember;

            A.CallTo(() =>
                _fileUploader.LoginAsync(A<string>._, A<string>._, A<SecureString>._, A<bool>._))
                    .Returns(Task.FromResult(true));

            _model.LoginCommand.Execute(_password);

            A.CallTo(() => _passwordManager.RemovePassword(A<string>._, A<string>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_InsecureConnection_Then_WarningDialogIsShown()
        {
            _model.WikiUrl = "http://Foo";
            _model.Username = "Bar";
            _model.RememberPassword = RememberPasswordOptions.DoNotRemember;

            _model.LoginCommand.Execute(_password);

            A.CallTo(() => _dialogs.ConfirmInsecureLoginDialog())
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_InsecureConnectionAndUserCancels_Then_LoginNotAttempted()
        {
            _model.WikiUrl = "http://Foo";
            _model.Username = "Bar";
            _model.RememberPassword = RememberPasswordOptions.DoNotRemember;
            A.CallTo(() => _dialogs.ConfirmInsecureLoginDialog()).Returns(false);

            _model.LoginCommand.Execute(_password);

            A.CallTo(() => _fileUploader.LoginAsync(A<string>._, A<string>._, A<SecureString>._, A<bool>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void When_InsecureConnectionAndUserOks_Then_LoginProceeds()
        {
            _model.WikiUrl = "http://Foo";
            _model.Username = "Bar";
            _model.RememberPassword = RememberPasswordOptions.DoNotRemember;
            A.CallTo(() => _dialogs.ConfirmInsecureLoginDialog()).Returns(true);

            _model.LoginCommand.Execute(_password);

            A.CallTo(() => _fileUploader.LoginAsync(A<string>._, A<string>._, A<SecureString>._, A<bool>._))
                .MustHaveHappened(1, Times.Exactly);
        }


    }
}
