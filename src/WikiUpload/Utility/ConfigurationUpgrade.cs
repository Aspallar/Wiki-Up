using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WikiUpload
{
    internal class ConfigurationUpgrade
    {
        private IHelpers _helpers;

        public ConfigurationUpgrade(IHelpers helpers)
        {
            _helpers = helpers;
        }

        public void UpgradePreviousConfiguration(string[] additionaFiles)
        {
            var currentFolder = _helpers.GetUserSettingsFolderName();
            CopyPreviousConfigurationTo(currentFolder, additionaFiles);
        }

        private void CopyPreviousConfigurationTo(string folderPath, IEnumerable<string> additionaFiles = null)
        {
            var parent = Path.GetDirectoryName(folderPath);
            var previousVersion = GetPreviousVersion(parent, folderPath);
            var minVersion = new Version(1,16,0,0);
            if (previousVersion != null && previousVersion >= minVersion)
            {
                var srcPath = parent + "\\" + previousVersion.ToString() + "\\";

                if (!_helpers.DirectoryExists(folderPath))
                    _helpers.CreateDirectory(folderPath);

                CopyFiles(srcPath, folderPath + "\\", ConcatenateFileNames("user.config", additionaFiles));
            }
        }

        private IEnumerable<string> ConcatenateFileNames(string firstFileNane, IEnumerable<string> otherFileNames)
        {
            yield return firstFileNane;
            if (otherFileNames != null)
            {
                foreach (var fileName in otherFileNames)
                    yield return fileName;
            }
        }

        private void CopyFiles(string srcPath, string destPath,IEnumerable<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                var fullPath = srcPath + fileName;
                if (_helpers.FileExists(fullPath))
                    _helpers.CopyFile(fullPath, destPath + fileName);
            }
        }

        private Version GetPreviousVersion(string parent, string currentFolder)
        {
            var currentVersion = new Version(Path.GetFileName(currentFolder));
            var versionFolder = new Regex(@"\\\d+\.\d+\.\d+\.\d+$");

            var previousVersions = _helpers.EnumerateDirectories(parent)
                .Where(folder => versionFolder.IsMatch(folder))
                .Select(folder => new Version(Path.GetFileName(folder)))
                .Where(version => version < currentVersion);

            if (previousVersions.Any())
                return previousVersions.Aggregate((maxVersion, nextVersion) => maxVersion >= nextVersion ? maxVersion : nextVersion);
            else
                return null;
        }

    }
}
