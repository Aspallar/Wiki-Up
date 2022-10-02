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
    public class RememberPasswordOptionsToIntConverterTests
    {
        private RememberPasswordOptionsToIntConverter _converter;

        [SetUp]
        public void SetUp()
        {
            _converter = new RememberPasswordOptionsToIntConverter();
        }

        [Test]
        public void Must_Return_Equivalent_Integer()
        {
            foreach (int value in Enum.GetValues(typeof(RememberPasswordOptions)))
            {
                var result = (int)_converter.Convert((RememberPasswordOptions)value, null, null, null);
                Assert.That(result, Is.EqualTo(value));
            }
        }

    }
}
