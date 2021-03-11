using NUnit.Framework;
using WikiUpload;

namespace Tests
{
    [TestFixture]
    public class FileExtensionsCollectionTests
    {
        [Test]
        public void When_Created_The_InitialisedFromString()
        {
            var fe = new FileExensionsCollection("foo;bar;");

            Assert.That(fe.Count, Is.EqualTo(2));
            Assert.That(fe[0], Is.EqualTo("foo"));
            Assert.That(fe[1], Is.EqualTo("bar"));
        }

        [Test]
        public void When_ConvertedToString_The_SemicolonSeparatedStringIsReturned()
        {
            var fe = new FileExensionsCollection();
            fe.Add("foo");
            fe.Add("bar");

            var result = fe.ToString();

            Assert.That(result, Is.EqualTo("foo;bar;"));
        }

        [Test]
        public void When_EmptyConvertedToString_The_EmpltyStringIsReturned()
        {
            var fe = new FileExensionsCollection();

            var result = fe.ToString();

            Assert.That(result, Is.EqualTo(""));
        }
    }
}
