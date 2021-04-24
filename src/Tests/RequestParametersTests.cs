using NUnit.Framework;
using System.Collections.Generic;
using WikiUpload;

namespace Tests
{
    [TestFixture]
    public class RequestParametersTests
    {
        [Test]
        public void When_Add_Then_ItemsAreAdded()
        {
            var expected = new List<KeyValuePair<string, string>>
            {
                new("a", "1"),
                new("b", "2"),
            };

            var reqParams = new RequestParameters();
            foreach (var item in expected)
                reqParams.Add(item.Key, item.Value);

            Assert.That(reqParams, Is.EquivalentTo(expected));
        }

        [Test]
        public void When_Empty_Then_ToStringIsEmpty()
        {
            var reqParams = new RequestParameters();

            Assert.That(reqParams.ToString(), Is.Empty);
        }

        [Test]
        public void When_SingleEntry_Then_ToStringIsCorrect()
        {
            var expected = "?a=1";
            var reqParams = new RequestParameters()
            {
                {"a", "1" }
            };

            Assert.That(reqParams.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void When_MultipleEntries_Then_ToStringIsCorrect()
        {
            var expected = "?a=1&b=2";
            var reqParams = new RequestParameters()
            {
                {"a", "1" },
                {"b", "2" },
            };

            Assert.That(reqParams.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void QueryStringIsUrlEncoded()
        {
            var expected = "?a=one+two&b=%2B%3D%26";
            var reqParams = new RequestParameters()
            {
                {"a", "one two" },
                {"b", "+=&" },
            };

            Assert.That(reqParams.ToString(), Is.EqualTo(expected));
        }

    }
}
