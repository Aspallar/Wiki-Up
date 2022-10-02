using FakeItEasy;
using NUnit.Framework;
using WikiUpload;

namespace Tests.MiscellaneousTests
{
    [TestFixture]
    public class VariablePageContentTests
    {
        private IHelpers _helpers;

        [SetUp]
        public void SetUp()
        {
            _helpers = A.Fake<IHelpers>();
        }

        [Test]
        public void When_BothContentFilesExist_Then_FileLevelFileIsUsed()
        {
            A.CallTo(() => _helpers.FileExists(@"c:\foo\foo.png.wikitext"))
                .Returns(true);
            A.CallTo(() => _helpers.FileExists(@"c:\foo\wikitext.wikitext"))
                .Returns(true);
            A.CallTo(() => _helpers.ReadAllText(@"c:\foo\foo.png.wikitext"))
                .Returns("File Contents <%-1>");
            var file = new UploadFile(@"c:\foo\foo.png");
            var pageContent = new VariablePageContent("wikitext", "default", _helpers);

            var result = pageContent.ExpandedContent(file);

            Assert.That(result, Is.EqualTo("File Contents foo.png"));
        }

        [Test]
        public void When_JustFolderContentFilesExists_Then_FoledereLevelFileIsUsed()
        {
            A.CallTo(() => _helpers.FileExists(@"c:\foo\wikitext.wikitext"))
                .Returns(true);
            A.CallTo(() => _helpers.ReadAllText(@"c:\foo\wikitext.wikitext"))
                .Returns("File Contents <%-1>");
            var file = new UploadFile(@"c:\foo\foo.png");
            var pageContent = new VariablePageContent("wikitext", "default", _helpers);

            var result = pageContent.ExpandedContent(file);

            Assert.That(result, Is.EqualTo("File Contents foo.png"));
        }

        [Test]
        public void When_NorContentFilesExists_Then_DefaulktContentIsUsed()
        {
            var file = new UploadFile(@"c:\foo\foo.png");
            var pageContent = new VariablePageContent("wikitext", "default <%-1>", _helpers);

            var result = pageContent.ExpandedContent(file);

            Assert.That(result, Is.EqualTo("default foo.png"));
        }
    }
}
