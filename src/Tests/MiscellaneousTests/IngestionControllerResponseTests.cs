using Newtonsoft.Json;
using NUnit.Framework;
using WikiUpload;

namespace Tests.MiscellaneousTests
{
    [TestFixture]
    public class IngestionControllerResponseTests
    {
        [Test]
        public void When_DeserializingAndStatusContainsHtml_Then_HtmlIsConvertedToText()
        {
            var json = "{\"success\": false, \"status\": \"<div><p>Foobar <b>is</b> here</p></div|>\"}";

            var result = JsonConvert.DeserializeObject<IngestionControllerResponse>(json);

            Assert.That(result.Status, Is.EqualTo("Foobar is here"));
        }

        [Test]
        public void When_DeserializingAndStatusContainsHtml_Then_WhitespaceIsStripped()
        {
            var json = "{\"success\": false, \"status\": \"<div><p>Foobar <b>is</b> here\n\n\n</p></div|>\"}";

            var result = JsonConvert.DeserializeObject<IngestionControllerResponse>(json);

            Assert.That(result.Status, Is.EqualTo("Foobar is here"));
        }

        [Test]
        public void When_DeserializingAndStatusIsNotHtml_Then_StatusIsSet()
        {
            var json = "{\"success\": false, \"status\": \"Foobar\n\n\"}";

            var result = JsonConvert.DeserializeObject<IngestionControllerResponse>(json);

            Assert.That(result.Status, Is.EqualTo("Foobar"));
        }

        [Test]
        public void When_DeserializingAndStatusIsEmpty_Then_StatusIsEmpty()
        {
            var json = "{\"success\": false, \"status\": \"\"}";

            var result = JsonConvert.DeserializeObject<IngestionControllerResponse>(json);

            Assert.That(result.Status, Is.EqualTo(""));
        }

        [Test]
        public void When_DeserializingAndStatusIsNull_Then_StatusIsNull()
        {
            var json = "{\"success\": false, \"status\": null }";

            var result = JsonConvert.DeserializeObject<IngestionControllerResponse>(json);

            Assert.That(result.Status, Is.Null);
        }

    }
}
