using FakeItEasy;
using NUnit.Framework;
using System;
using WikiUpload;

namespace Tests.MiscellaneousTests
{
    [TestFixture]
    public class ConfigurationUpgradeTests
    {
        const string path = @"c:\a\b";
        const string versionPath = @"\1.16.1.0";
        const string previousVrsionPath = @"\1.16.0.0";
        const string otherFile = "OtherFile.txt";
        string[] otherFiles = new string[] { otherFile };

        private IHelpers _helpers;
        private ConfigurationUpgrade _configurationUpgrade;

        [SetUp]
        public void SetUp()
        {
            _helpers = A.Fake<IHelpers>();
            _configurationUpgrade = new ConfigurationUpgrade(_helpers);

            A.CallTo(() => _helpers.GetUserSettingsFolderName()).Returns(path + versionPath);
            A.CallTo(() => _helpers.FileExists(A<string>.Ignored)).Returns(true);
            A.CallTo(() => _helpers.DirectoryExists(A<string>.Ignored)).Returns(false);
        }

        [Test]
        public void Later_Versions_Are_Ignored()
        {
            A.CallTo(() => _helpers.EnumerateDirectories(path)).Returns(new string[]
            {
                path + versionPath,
                path + previousVrsionPath,
                path + "\\2.15.1.0",
                path + "\\Foobar.txt"
            });

            _configurationUpgrade.UpgradePreviousConfiguration(otherFiles);

            const string userConfig = "\\user.config";
            const string expectedUserConfigDest = path + versionPath + userConfig;
            const string expectedUserConfigSource = path + previousVrsionPath + userConfig;
            A.CallTo(() => _helpers.CopyFile(expectedUserConfigSource, expectedUserConfigDest))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void Correct_Version_Is_used()
        {
            A.CallTo(() => _helpers.EnumerateDirectories(path)).Returns(new string[]
            {
                path + versionPath,
                path + previousVrsionPath,
                path + "\\1.15.1.0",
                path + "\\Foobar.txt"
            });

            _configurationUpgrade.UpgradePreviousConfiguration(otherFiles);

            A.CallTo(() => _helpers.CreateDirectory(path + versionPath))
                .MustHaveHappened();

            A.CallTo(() => _helpers.CopyFile(A<string>._, A<string>._))
                .MustHaveHappened(2, Times.Exactly);

            const string userConfig = "\\user.config";
            const string expectedUserConfigDest = path + versionPath + userConfig;
            const string expectedUserConfigSource = path + previousVrsionPath + userConfig;
            A.CallTo(() => _helpers.CopyFile(expectedUserConfigSource, expectedUserConfigDest))
                .MustHaveHappened(1, Times.Exactly);

            const string expectedOtherDest = path + versionPath + "\\" + otherFile;
            const string expectedOtherSource = path + previousVrsionPath + "\\" + otherFile;
            A.CallTo(() => _helpers.CopyFile(expectedOtherSource, expectedOtherDest))
                .MustHaveHappened(1, Times.Exactly);
        }


        [Test]
        public void Versions_Below_Minimum_Are_Ignored()
        {
            A.CallTo(() => _helpers.EnumerateDirectories(path)).Returns(new string[]
            {
                path + versionPath,
                path + "\\1.15.1.0",
            });

            _configurationUpgrade.UpgradePreviousConfiguration(otherFiles);

            A.CallTo(() => _helpers.CopyFile(A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void When_File_Does_Not_Exist_Then_File_Is_Not_Copied()
        {
            A.CallTo(() => _helpers.FileExists(A<string>.Ignored)).Returns(false);
            A.CallTo(() => _helpers.EnumerateDirectories(path)).Returns(new string[]
            {
                path + versionPath,
                path + previousVrsionPath,
            });

            _configurationUpgrade.UpgradePreviousConfiguration(otherFiles);

            A.CallTo(() => _helpers.CopyFile(A<string>.Ignored, A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public void When_No_Previous_Version_No_Files_No_Files_Are_Copied()
        {
            A.CallTo(() => _helpers.EnumerateDirectories(A<string>.Ignored)).Returns(Array.Empty<string>());

            _configurationUpgrade.UpgradePreviousConfiguration(otherFiles);

            A.CallTo(() => _helpers.CopyFile(A<string>.Ignored, A<string>.Ignored))
                .MustNotHaveHappened();
        }


    }
}
