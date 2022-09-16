using FakeItEasy;
using NUnit.Framework;
using System.Collections;
using System.Text;
using WikiUpload;
using WikiUpload.Properties;

namespace Tests.ValidationRuleTests
{
    [TestFixture]
    public class WikiFileTitleValidationRuleTests
    {
        private WikiFileTitleValidationRule _validationRule;

        [SetUp]
        public void SetUp()
        {
            _validationRule = new WikiFileTitleValidationRule();
        }


        [Test]
        public void Valid_Name_Gives_Valid_Result()
        {
            string[] tests = { "a.a", "a.a.a", "a..a" };
            ValidateAndAssertValidReturn(tests);
        }


        [Test]
        public void Name_Cannot_Contain_Invalid_Characters()
        {
            char[] invalidCharacters = { '#', '<', '>', '[', ']', '|', '{', '}', ':' };

            var expectedErrorContent = Resources.EditUploadFileNameErrorInvalidCharacters;
            foreach (var ch in invalidCharacters)
            {
                ValidateAndAssertErrorReturn("a" + ch + "a.a", expectedErrorContent);
                ValidateAndAssertErrorReturn(ch + "a.a", expectedErrorContent);
                ValidateAndAssertErrorReturn("a.a" + ch, expectedErrorContent);
            }
        }

        [Test]
        public void Name_Cannot_Be_Relative_Path()
        {
            string[] relativePaths = { ".", ".." };
            ValidateAndAssertErrorReturn(relativePaths, Resources.EditUploadFileNameErrorIsRelativePath);
        }

        [Test]
        public void Name_Cannot_Contain_Relative_Path()
        {
            string[] relativePaths = { "/./", "/../" };

            foreach (var path in relativePaths)
                ValidateAndAssertErrorReturn("foo" + path + "bar", Resources.EditUploadFileNameErrorContainsRelativePath);
        }

        [Test]
        public void Name_Cannot_Start_With_Relative_Path()
        {
            string[] relativePaths = { "./", "../" };

            foreach (var path in relativePaths)
                ValidateAndAssertErrorReturn(path + "bar", Resources.EditUploadFileNameErrorStartsWithRelativePath);
        }

        [Test]
        public void Name_Cannot_End_With_Relative_Path()
        {
            string[] relativePaths = { "/.", "/.." };

            foreach (var path in relativePaths)
                ValidateAndAssertErrorReturn("foo" + path, Resources.EditUploadFileNameErrorEndsWithRelativePath);
        }

        [Test]
        public void Name_Cannot_Start_With_Whitespace()
        {
            string[] tests = { "_foo.a", " bar.a", " .a", "_.a" };
            ValidateAndAssertErrorReturn(tests, Resources.EditUploadFileNameErrorWhitespaceAtStart);
        }

        [Test]
        public void Name_Cannot_End_With_Whitespace()
        {
            string[] tests = { "foo.a_", "bar.a ", ".a ", ".a_" };
            ValidateAndAssertErrorReturn(tests, Resources.EditUploadFileNameErrorWhitespaceAtEnd);
        }

        [Test]
        public void Name_Cannot_Contain_Three_Consectutive_Tildes()
        {
            string[] tests = { "aa~~~bb.a", "~~~.a", "a.~~~", "~~~~~.a" };
            ValidateAndAssertErrorReturn(tests, Resources.EditUploadFileNameErrorThreeTildes);
        }

        [Test]
        public void Name_Cannot_Contain_Two_Consectutive_Whitespace()
        {
            string[] tests = { "a  a.a", "a_ a.a", "a _a.a", "a__a.a" };
            ValidateAndAssertErrorReturn(tests, Resources.EditUploadFileNameErrorConsecutiveSpaces);
        }

        [Test]
        public void Name_Cannot_Contain_Url_Escape()
        {
            string[] tests = { "a%A1.a", "a%a1.a", "aa%01aa.a" };
            ValidateAndAssertErrorReturn(tests, Resources.EditUploadFileNameErrorUrlEscape);
        }

        [Test]
        public void Name_Can_Contain_Percent_If_Not_Url_Escape()
        {
            string[] tests = { "%A.a", "%az.a", "%.a" };
            ValidateAndAssertValidReturn(tests);
        }

        [Test]
        public void Name_Cannot_Be_Longer_Than_255_Bytes()
        {
            string test = new string('a', 254) + ".a";
            System.Diagnostics.Debug.Assert(Encoding.UTF8.GetBytes(test).Length == 256);
            ValidateAndAssertErrorReturn(test, Resources.EditUploadFileNameErrorTooLong);
        }

        [Test]
        public void Name_Can_Be_Up_To_255_Bytes_In_Length()
        {
            string[] tests = { new string('a', 253) + ".a" };
            System.Diagnostics.Debug.Assert(Encoding.UTF8.GetBytes(tests[0]).Length == 255);
            ValidateAndAssertValidReturn(tests);
        }

        [Test]
        public void Name_Must_Have_Extension()
        {
            string[] tests = { "foo", "foo." };
            ValidateAndAssertErrorReturn(tests, Resources.EditUploadFileNameErrorMustHaveExension);
        }

        [Test]
        public void Name_Must_Have_File_Name()
        {
            string test =  ".a";
            ValidateAndAssertErrorReturn(test, Resources.EditUploadFileNameErrorMustHaveFileName);
        }

        private void ValidateAndAssertErrorReturn(string testString, string expectedContent = null)
        {
            var result = _validationRule.Validate(testString, null);
            Assert.That(result.IsValid, Is.False);

            if (expectedContent != null)
                Assert.That(result.ErrorContent, Is.EqualTo(expectedContent));
        }

        private void ValidateAndAssertErrorReturn(IList testStrings, string expectedContent = null)
        {
            foreach (var testString in testStrings)
            {
                var result = _validationRule.Validate(testString, null);
                Assert.That(result.IsValid, Is.False);
                if (expectedContent != null)
                    Assert.That(result.ErrorContent, Is.EqualTo(expectedContent));
            }
        }

        private void ValidateAndAssertValidReturn(IList testStrings)
        {
            foreach (var testString in testStrings)
            {
                var result = _validationRule.Validate(testString, null);
                Assert.That(result.IsValid, Is.True);
            }
        }
    }
}
            