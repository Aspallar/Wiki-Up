using FakeItEasy;
using MahApps.Metro.IconPacks;
using NUnit.Framework;
using System;
using System.Windows;
using WikiUpload;

namespace Tests.ValueConverterTests
{
    [TestFixture]
    public class BooleanToVisiblityConverterTests
    {
        private BooleanToVisiblityConverter _converter;


        [SetUp]
        public void SetUp()
        {
            _converter = new BooleanToVisiblityConverter();
        }

        [Test]
        public void When_ParameterIsNull_ThenTrueMeansHidden()
        {
            var visibility = _converter.Convert(true, null, null, null);
            Assert.That(visibility, Is.EqualTo(Visibility.Hidden));
        }

        [Test]
        public void When_ParameterIsNull_ThenFalseMeansVisible()
        {
            var visibility = _converter.Convert(false, null, null, null);
            Assert.That(visibility, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void When_ParameterIsNotNull_ThenTrueMeansVisible()
        {
            var visibility = _converter.Convert(true, null, this, null);
            Assert.That(visibility, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void When_ParameterIsNotNull_ThenFalseMeansHidden()
        {
            var visibility = _converter.Convert(false, null, this, null);
            Assert.That(visibility, Is.EqualTo(Visibility.Hidden));
        }

        [Test]
        public void When_ValueIsNotBool_ThenFalseIsUsed()
        {
            var visibility = _converter.Convert(123, null, this, null);
            Assert.That(visibility, Is.EqualTo(Visibility.Hidden));
        }

        [Test]
        public void When_ValueIsNull_ThenFalseIsUsed()
        {
            var visibility = _converter.Convert(null, null, this, null);
            Assert.That(visibility, Is.EqualTo(Visibility.Hidden));
        }

    }
}
