using NUnit.Framework;
using System.Collections.Generic;
using WikiUpload;

namespace Tests.MiscellaneousTests
{
    [TestFixture]
    public class PermittedFilesTests
    {

        [Test]
        public void When_ThereArePermittedFiles_Then_OnlySpecifiedFilesArePermitted()
        {
            var  permittedfiles = CreatePermittedFiles(new List<string> { "aaa", "bbb" });
            Assert.That(permittedfiles.IsPermitted("foo.aaa"), Is.True);
            Assert.That(permittedfiles.IsPermitted("foo.bbb"), Is.True);
            Assert.That(permittedfiles.IsPermitted("foo.zzz"), Is.False);
            Assert.That(permittedfiles.IsPermitted("foo"), Is.False);
            Assert.That(permittedfiles.IsPermitted(""), Is.False);
        }

        [Test]
        public void When_ThereAreNoPermittedFiles_Then_AllFileaArePermitted()
        {
            var permittedfiles = new PermittedFiles();
            Assert.That(permittedfiles.IsPermitted("foo.aaa"), Is.True);
            Assert.That(permittedfiles.IsPermitted("foo.bbb"), Is.True);
            Assert.That(permittedfiles.IsPermitted("foo.zzz"), Is.True);
            Assert.That(permittedfiles.IsPermitted("foo"), Is.True);
            Assert.That(permittedfiles.IsPermitted(""), Is.True);
        }

        [Test]
        public void IsPermitted_Is_CaseInsensitive()
        {
            var permittedfiles = CreatePermittedFiles(new List<string> { "aaa" });
            Assert.That(permittedfiles.IsPermitted("foo.AAA"), Is.True);
        }

        [Test]
        public void GetExtensions_Returns_AllExtensionsWithPeriod()
        {
            var permittedfiles = CreatePermittedFiles(new List<string> { "aaa", "bbb" });
            var result = permittedfiles.GetExtensions();
            Assert.That(result.Length, Is.EqualTo(2));
            Assert.That(result, Does.Contain(".aaa"));
            Assert.That(result, Does.Contain(".bbb"));
        }

        [Test]
        public void When_NoPermittedFiles_Then_GetExtensions_Returns_EmptyArray()
        {
            var permittedfiles = new PermittedFiles();
            var result = permittedfiles.GetExtensions();
            Assert.That(result.Length, Is.Zero);
        }

        [Test]
        public void Get_Extensions_Returns_Copy()
        {
            var permittedfiles = CreatePermittedFiles(new List<string> { "aaa", "bbb" });
            var result = permittedfiles.GetExtensions();
            result[0] = ".zzz";
            result[1] = ".yyy";
            Assert.That(permittedfiles.IsPermitted("foo.aaa"), Is.True);
            Assert.That(permittedfiles.IsPermitted("foo.bbb"), Is.True);
            Assert.That(permittedfiles.IsPermitted("foo.zzz"), Is.False);
        }

        private PermittedFiles CreatePermittedFiles(IEnumerable<string> extensions)
        {
            var permittedFiles = new PermittedFiles();
            foreach (var ext in extensions)
                permittedFiles.Add(ext);
            return permittedFiles;

        }
    }
}
