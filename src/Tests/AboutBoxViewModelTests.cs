using FakeItEasy;
using NUnit.Framework;
using System.Threading;
using System.Windows;
using WikiUpload;

namespace Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class AboutBoxViewModelTests
    {
        private const string Version = "bar";
        private const string Copyright = "foo";

        private IHelpers _helpers;
        private AboutBoxViewModel _model;

        [SetUp]
        public void Setuo()
        {
            _helpers = A.Fake<IHelpers>();
            A.CallTo(() => _helpers.ApplicationInformation)
                .Returns((Copyright, Version));
            _model = new AboutBoxViewModel(new Window(), _helpers);
        }

        [Test]
        public void When_Created_Then_VersionIsSet()
        {
            Assert.That(_model.VersionText, Does.EndWith(Version));
            Assert.That(_model.VersionText, Does.StartWith("Version "));
        }

        [Test]
        public void When_Created_Then_CopyrightIsSet()
        {
            Assert.That(_model.CopyrightText, Is.EqualTo(Copyright));
        }

        [Test]
        public void When_LaunchWebsiteIsExecuted_Then_WebsiteIsLaunched()
        {
            _model.LaunchWebSiteCommand.Execute(null);

            A.CallTo(()=>_helpers.LaunchProcess("https://github.com/Aspallar/Wiki-Up"))
                .MustHaveHappened(1, Times.Exactly);
        }

    }
}
