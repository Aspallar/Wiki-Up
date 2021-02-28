using NUnit.Framework;
using System.Threading;
using System.Windows;
using WikiUpload;

namespace Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class ErrorMessageViewModelTests
    {
        [Test]
        public void When_ExceptionMessageIsNull_Then_ExceptionMTextIsCollaped()
        {
            var model = new ErrorMessageViewModel(new Window());
            model.SubMessage = null;

            Assert.That(model.ExceptionVisibility, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void When_ExceptionMessageIsEmpty_Then_ExceptionMTextIsCollaped()
        {
            var model = new ErrorMessageViewModel(new Window());
            model.SubMessage = string.Empty;

            Assert.That(model.ExceptionVisibility, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void When_ExceptionMessageHasContent_Then_ExceptionMTextIsVisible()
        {
            var model = new ErrorMessageViewModel(new Window());
            model.SubMessage = "foobar";

            Assert.That(model.ExceptionVisibility, Is.EqualTo(Visibility.Visible));
        }
    }
}
