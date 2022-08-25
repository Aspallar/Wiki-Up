using NUnit.Framework;
using WikiUpload;

namespace Tests.ValueConverterTests
{
    [TestFixture]
    public class NotConverterTests
    {
        private NotConverter _converter;

        [SetUp]
        public void SetUp()
        {
            _converter = new NotConverter();
        }

        [Test]
        public void When_True_Then_False()
        {
            var result = (bool)_converter.Convert(true, null, null, null);
            Assert.That(result, Is.False);
        }

        [Test]
        public void When_False_Then_True()
        {
            var result = (bool)_converter.Convert(false, null, null, null);
            Assert.That(result, Is.True);
        }

        [Test]
        public void When_NotBoolean_Then_True()
        {
            var result = (bool)_converter.Convert(this, null, null, null);
            Assert.That(result, Is.True);
        }

        [Test]
        public void When_Null_Then_True()
        {
            var result = (bool)_converter.Convert(null, null, null, null);
            Assert.That(result, Is.True);
        }

    }
}
