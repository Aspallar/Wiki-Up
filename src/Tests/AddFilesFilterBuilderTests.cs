using NUnit.Framework;
using System;
using WikiUpload;

namespace Tests
{
    [TestFixture]
    public class AddFilesFilterBuilderTests
    {
        [Test]
        public void When_NoPermittedFiles_Then_FilterIsForAllFiles()
        {
            var expected = "Image Files|*.foo;*.bar|Other Files|*.odt;*.ods;*.odp;*.odg;*.odc;*.odf;*.odi;*.odm;*.ogg;*.ogv;*.oga|All Files|*.*";

            var result = AddFilesFilterBuilder.Build(Array.Empty<string>(), "foo;bar");

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void When_NoPermittedFilesAndNoImages_Then_FilterDoesNotContainImageEntry()
        {
            var expected = "Other Files|*.odt;*.ods;*.odp;*.odg;*.odc;*.odf;*.odi;*.odm;*.ogg;*.ogv;*.oga|All Files|*.*";

            var result = AddFilesFilterBuilder.Build(Array.Empty<string>(), "");

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void When_NoImages_Then_FilterDoesNotContainImageEntry()
        {
            var expected = "Other Files|*.odt|All Files|*.*";

            var result = AddFilesFilterBuilder.Build(new string[] { ".odt" }, "");

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void When_AllPermittedFilesAreImages_Then_FilterDoesNotContainOtherFileEntry()
        {
            var expected = "Image Files|*.foo;*.bar|All Files|*.*";

            var result = AddFilesFilterBuilder.Build(new string[] { ".foo", ".bar" }, "foo;bar");

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void When_PermittedFiles_Then_OnlyPermittedFilesAreInFilter()
        {
            var expected = "Image Files|*.foo|Other Files|*.foobar|All Files|*.*";

            var result = AddFilesFilterBuilder.Build(new string[] { ".foo", ".foobar" }, "foo;bar" );

            Assert.That(result, Is.EqualTo(expected));
        }

    }
}
