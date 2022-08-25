using MahApps.Metro.IconPacks;
using NUnit.Framework;
using System;
using WikiUpload;

namespace Tests.ValueConverterTests
{
    [TestFixture]
    public class UploadFileStatusToKindConverterTests
    {
        private UploadFileStatusToKindConverter _converter;

        [SetUp]
        public void SetUp()
        {
            _converter = new UploadFileStatusToKindConverter();
        }


        [Test]
        public void When_StatusIsUploading_Then_SpinnerSolid()
        {
            TestConverterResult(UploadFileStatus.Uploading, PackIconFontAwesomeKind.SpinnerSolid);
        }

        [Test]
        public void When_StatusIsDelaying_Then_SpinnerSolid()
        {
            TestConverterResult(UploadFileStatus.Delaying, PackIconFontAwesomeKind.SpinnerSolid);
        }

        [Test]
        public void When_StatusIsWarning_Then_ExclamationTriangleSolid()
        {
            TestConverterResult(UploadFileStatus.Warning, PackIconFontAwesomeKind.ExclamationTriangleSolid);
        }

        [Test]
        public void When_StatusIsError_Then_TimesCircleRegular()
        {
            TestConverterResult(UploadFileStatus.Error, PackIconFontAwesomeKind.TimesCircleRegular);
        }

        [Test]
        public void When_StatusIsWaiting_And_FileIsVideo_Then_FilmSolid()
        {
            var values = new object[] { UploadFileStatus.Waiting, true };
            var result = (PackIconFontAwesomeKind)_converter.Convert(values, null, null, null);
            Assert.That(result, Is.EqualTo(PackIconFontAwesomeKind.FilmSolid));
        }

        [Test]
        public void When_StatusIsWaiting_And_FileIsNotVideo_Then_AngleUpSolid()
        {
            var values = new object[] { UploadFileStatus.Waiting, false };
            var result = (PackIconFontAwesomeKind)_converter.Convert(values, null, null, null);
            Assert.That(result, Is.EqualTo(PackIconFontAwesomeKind.AngleUpSolid));
        }

        [Test]
        public void When_InvalidValues_Then_ExceptionIsThrown()
        {
            var values = new object[] { 1000 };
            Assert.Catch<Exception>(() => _converter.Convert(values, null, null, null));

            values = Array.Empty<object>();
            Assert.Catch<Exception>(() => _converter.Convert(values, null, null, null));

            values = new object[] { UploadFileStatus.Waiting, 10 };
            Assert.Catch<Exception>(() => _converter.Convert(values, null, null, null));
        }

        [Test]
        public void When_StatusOsWaiting_Then_IsVideoMustBeSupplied()
        {
            var values = new object[] { UploadFileStatus.Waiting };
            Assert.Throws<IndexOutOfRangeException>(() => _converter.Convert(values, null, null, null));
        }

        private void TestConverterResult(UploadFileStatus status, PackIconFontAwesomeKind expectedKind)
        {
            var values = new object[] { status };
            var valuesTrue = new object[] { status, true };
            var valuesFalse = new object[] { status, false };

            var resultNull = (PackIconFontAwesomeKind)_converter.Convert(values, null, null, null);
            var resultTrue = (PackIconFontAwesomeKind)_converter.Convert(valuesTrue, null, null, null);
            var resultFalse = (PackIconFontAwesomeKind)_converter.Convert(valuesFalse, null, null, null);

            Assert.That(resultNull, Is.EqualTo(expectedKind));
            Assert.That(resultTrue, Is.EqualTo(expectedKind));
            Assert.That(resultFalse, Is.EqualTo(expectedKind));
        }

    }
}
