﻿using NUnit.Framework;
using WikiUpload;

namespace Tests.MiscellaneousTests
{
    [TestFixture]
    public class VariableContentTests
    {
        const string testUploadFilePath = "Dummy.png";
        const string testUploadFileFileName = "Dummy";
        const string testUploadFileExtension = ".png";

        [Test]
        public void When_NoVariables_Then_HasVariablesIsFalse()
        {
            var variableContent = new VariableContent("Once upon a time");

            Assert.That(variableContent.HasVariables, Is.False);
        }

        [Test]
        public void When_InvalidVariables_Then_NoSubstitution()
        {
            const string test = "Once upon <%foobar> a time";
            var variableContent = new VariableContent(test);
            var file = new UploadFile(@"c:\foobar");

            Assert.That(variableContent.ExpandedContent(file), Is.EqualTo(test));
        }

        [Test]
        public void When_Variables_Then_HasVariablesIsTrue()
        {
            var variableContent = new VariableContent("<%4>");

            Assert.That(variableContent.HasVariables, Is.True);
        }

        [Test]
        public void When_NegativeVariables_Then_HasVariablesIsTrue()
        {
            var variableContent = new VariableContent("<%-4>");

            Assert.That(variableContent.HasVariables, Is.True);
        }

        [Test]
        public void When_ExpansionIndexNotValidInteger_Then_SubstitutionNatDone()
        {
            var test = "Once upon<%999999999999999999999999999999999999999999999999999999999999> a time";
            var file = new UploadFile("foobar");

            var variableContent = new VariableContent(test);

            Assert.That(variableContent.ExpandedContent(file), Is.EqualTo(test));
        }

        [Test]
        public void When_FilenameExpansion_Then_FilenameWithouExtensionIsSubstituted()
        {
            var test = "<%filename>";
            var file = CreateUploadFile(@"c:\foo\foobar.png");

            var variableContent = new VariableContent(test);

            Assert.That(variableContent.ExpandedContent(file), Is.EqualTo("foobar"));
        }

        [Test]
        public void When_UploadFilenameExpansion_Then_UploadFilenameWithouExtensionIsSubstituted()
        {
            var test = "<%uploadfilename>";
            var file = CreateUploadFile(@"c:\foo\foobar.png");

            var variableContent = new VariableContent(test);

            Assert.That(variableContent.ExpandedContent(file), Is.EqualTo(testUploadFileFileName));
        }

        [Test]
        public void When_UploadExtensionExpansion_Then_UploadFilenameExtensionIsSubstituted()
        {
            var test = "<%uploadextension>";
            var file = CreateUploadFile(@"c:\foo\foobar.png");

            var variableContent = new VariableContent(test);

            Assert.That(variableContent.ExpandedContent(file), Is.EqualTo(testUploadFileExtension));
        }

        [Test]
        public void When_NegativeExpansionIndexNotValidInteger_Then_SubstitutionNatDone()
        {
            var test = "Once upon<%-999999999999999999999999999999999999999999999999999999999999> a time";
            var file = CreateUploadFile("foobar");

            var variableContent = new VariableContent(test);

            Assert.That(variableContent.ExpandedContent(file), Is.EqualTo(test));
        }

        [Test]
        public void When_ExpansionIndexNotInRange_Then_EmptyStringIsSubstituted()
        {
            var test = "<%4>";
            var file = CreateUploadFile(@"c:\a\b.foo");

            var variableContent = new VariableContent(test);

            Assert.That(variableContent.ExpandedContent(file), Is.Empty);
        }

        [Test]
        public void When_NegativeExpansionIndexNotInRange_Then_EmptyStringIsSubstituted()
        {
            var test = "<%-4>";
            var file = CreateUploadFile(@"c:\a\b.foo");

            var variableContent = new VariableContent(test);

            Assert.That(variableContent.ExpandedContent(file), Is.Empty);
        }

        [Test]
        public void When_ExpansionIndexIsZeroe_Then_FullPathIsSubstituted()
        {
            const string fileName = @"c:\a\b.foo";
            var test = "<%0>";
            var file = CreateUploadFile(fileName);

            var variableContent = new VariableContent(test);

            Assert.That(variableContent.ExpandedContent(file), Is.EqualTo(fileName));
        }

        [Test]
        public void When_Expansionse_Then_ExpansionsDone()
        {
            var test = "once <%1> upon <%2> a <%3> z <%4>";
            var expected = "once c: upon two a three z four.foo";
            var file = CreateUploadFile(@"c:\two\three\four.foo");

            var variableContent = new VariableContent(test);

            Assert.That(variableContent.ExpandedContent(file), Is.EqualTo(expected));
        }

        [Test]
        public void When_NegativeExpansionse_Then_ExpansionsDone()
        {
            var test = "once <%-1> upon <%-2> a <%-3> z <%-4>";
            var file = CreateUploadFile(@"c:\three\two\one.foo");
            var expected = "once one.foo upon two a three z c:";

            var variableContent = new VariableContent(test);

            Assert.That(variableContent.ExpandedContent(file), Is.EqualTo(expected));
        }

        private UploadFile CreateUploadFile(string path)
            => new UploadFile(path) { UploadFileName = testUploadFilePath };
    }
}
