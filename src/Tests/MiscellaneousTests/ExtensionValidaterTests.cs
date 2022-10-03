using NUnit.Framework;
using System.IO;
using WikiUpload;

namespace Tests.MiscellaneousTests
{
    [TestFixture]
    public class ExtensionValidaterTests
    {
        private IExtensionValidater _extensionValidator;

        [SetUp]
        public void SetUp()
        {
            _extensionValidator = new ExtensionValidater();
        }


        [Test]
        public void When_ValidExtension_The_IsValidIsTrue()
        {
            var result = _extensionValidator.IsValid("foo");
            Assert.That(result, Is.True);
        }


        [Test]
        public void When_ExtensionContainsInvalidFileSystemCharacters_Then_IsValidIsFalse()
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var ch in invalidChars)
            {
                string test = "foo" + ch + "bar";
                var result = _extensionValidator.IsValid(test);
                Assert.That(result, Is.False);
            }
        }

        [Test]
        public void When_ExtensionContainsSemiColon_Then_IsValidIsFalse()
        {
            var result = _extensionValidator.IsValid(";");
            Assert.That(result, Is.False);
        }

    }
}
