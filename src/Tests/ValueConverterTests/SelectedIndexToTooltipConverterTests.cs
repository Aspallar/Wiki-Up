using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiUpload;

namespace Tests.ValueConverterTests
{
    [TestFixture]
    public class SelectedIndexToTooltipConverterTests
    {
        private SelectedIndexToTooltipConverter _converter;

        [SetUp]
        public void SetUp()
        {
            _converter = new SelectedIndexToTooltipConverter();
        }

        [Test]
        public void When_NothingSelected_Then_RemoveAlFilesTooltip()
        {
            var expected = WikiUpload.Properties.Resources.RemoveAllFilesTooltip;
            var result = (string)_converter.Convert(-1, null, null, null);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void When_SomethingSelected_Then_RemoveSelectedFilesTooltip()
        {
            var expected = WikiUpload.Properties.Resources.RemoveSelectedFilesTooltip;
            var result = (string)_converter.Convert(0, null, null, null);
            Assert.That(result, Is.EqualTo(expected));
        }

    }
}
