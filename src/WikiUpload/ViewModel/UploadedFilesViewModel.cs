using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using WikiUpload.Properties;

namespace WikiUpload
{
    internal class UploadedFilesViewModel : BaseViewModel
    {
        private readonly IFileUploader _fileUploader;
        private readonly IHelpers _helpers;
        private readonly IDialogManager _dialogManager;
        private readonly IUploadListSerializer _uploadFileSerializer;

        public UploadedFilesViewModel(IFileUploader fileUploader, IHelpers helpers, IDialogManager dialogManager, IUploadListSerializer uploadFileSerializer)
        {
            _fileUploader = fileUploader;
            _helpers = helpers;
            _dialogManager = dialogManager;
            _uploadFileSerializer = uploadFileSerializer;

            UploadedFileSelectedIndex = -1;
            UploadedFiles = new UploadList(_helpers);
            UploadedFilesView = CollectionViewSource.GetDefaultView(UploadedFiles);
            LaunchFilePageCommand = new RelayParameterizedCommand((file) => LaunchFilePage((UploadFile)file));
            SortOrderCommand = new RelayParameterizedCommand((sort) => SortOrder((SortingOptions)sort));
            RemoveFilesCommand = new RelayParameterizedCommand(RemoveFiles);
            RemoveSelectedFilesCommand = new RelayParameterizedCommand((selectedItems) => RemoveSelectedFiles((IList)selectedItems));
            ClearSelectionCommand = new RelayCommand(() => UploadedFileSelectedIndex = -1);
            RemoveAllFilesCommand = new RelayCommand(RemoveAllFiles);

            CopyToClipboardCommand = new RelayCommand(CopyToClipboard);
            CopyTextToClipboardCommand = new RelayParameterizedCommand((selectedItems) => CopyTextToClipboard((IList)selectedItems));
            CopyFileToClipboardCommand = new RelayParameterizedCommand((selectedItems) => CopyFileToClipboard((IList)selectedItems));
            CopyWikilinkToClipboardCommand = new RelayParameterizedCommand((selectedItems) => CopyWikilinkToClipboard((IList)selectedItems));

            ActivateMainWindowCommand = new RelayCommand(() => _helpers.ActivateMainWindow());

            SaveLoadCommand = new RelayCommand(SaveLoad);
            SaveUploadedFilesCommand = new RelayCommand(SaveUploadedFiles);
            LoadUploadedFilesCommand = new RelayCommand(LoadUploadedFiles);
        }

        public int UploadedFileSelectedIndex { get; set; }
        public UploadList UploadedFiles { get; private set; }
        public ICollectionView UploadedFilesView { get; }
        public SortingOptions SortingOption { get; set; }
        public bool IsConfirmRemoveAllPopupOpen { get; set; }
        public bool IsCopiedPopupOpen { get; set; }
        public bool IsUnsortedFocused { get; set; }
        public bool IsAcsendingFocused { get; set; }
        public bool IsDescendingFocused { get; set; }
        public bool IsChooseCopyTypePopupOpen { get; set; }
        public bool IsSaveLoadPopupOpen { get; set; }
        public bool IsMustSignInPopupOpen { get; set; }

        public ICommand ClearSelectionCommand { get; }
        
        public ICommand ActivateMainWindowCommand { get; }

        public ICommand LaunchFilePageCommand { get; }
        private void LaunchFilePage(UploadFile file)
        {
            if (file != null)
            {
                var url = _fileUploader.FileUrl(file.UploadFileName);
                _helpers.LaunchProcess(url);
            }
        }

        public ICommand SaveLoadCommand { get; }
        private void SaveLoad()
        {
            if (_fileUploader.IsLoggedIn)
                IsSaveLoadPopupOpen = true;
            else
                IsMustSignInPopupOpen = true;
        }

        public ICommand SaveUploadedFilesCommand { get; }
        private void SaveUploadedFiles()
        {
            var result = _dialogManager.SaveUploadListDialog();
            if (result.Ok)
            {
                try
                {
                    _uploadFileSerializer.Serialize(result.Path, UploadedFiles);
                }
                catch (Exception ex)
                {
                    _dialogManager.ErrorMessage(Resources.CantSaveUploadListMessage, ex);
                }
            }
        }

        public ICommand LoadUploadedFilesCommand { get; }
        private void LoadUploadedFiles()
        {
            var result = _dialogManager.LoadUploadListDialog();
            if (result.Ok)
            {
                try
                {
                    var files = _uploadFileSerializer.Deserialize(result.Path);
                    UploadedFiles.Clear();
                    UploadedFiles.AddRange(files);
                }
                catch (Exception ex)
                {
                    _dialogManager.ErrorMessage(Resources.CantReadUploadListMessage, ex);
                }
            }
        }

        public ICommand RemoveFilesCommand { get; }
        private void RemoveFiles(object selectedItems)
        {
            IList selected = (IList)selectedItems;
            if (selected.Count == 0)
                IsConfirmRemoveAllPopupOpen = true;
            else
                RemoveSelectedFiles(selected);
        }

        public ICommand RemoveSelectedFilesCommand { get; }
        private void RemoveSelectedFiles(IList selectedItems)
        {
            UploadedFiles.RemoveRange(selectedItems.OfType<UploadFile>().ToList());
        }

        public ICommand RemoveAllFilesCommand { get; }
        private void RemoveAllFiles()
        {
            UploadedFiles.Clear();
            IsConfirmRemoveAllPopupOpen = false;
        }

        public ICommand CopyToClipboardCommand { get; }
        private void CopyToClipboard()
        {
            IsChooseCopyTypePopupOpen = true;
        }

        public ICommand CopyTextToClipboardCommand { get; }
        private void CopyTextToClipboard(IList selectedItems)
        {
            IsChooseCopyTypePopupOpen = false;
            if (UploadedFiles.Count > 0)
                SetClipboardText("{0}", selectedItems);
        }

        public ICommand CopyFileToClipboardCommand { get; }
        private void CopyFileToClipboard(IList selectedItems)
        {
            IsChooseCopyTypePopupOpen = false;
            if (UploadedFiles.Count > 0)
            {
                string format = _fileUploader.SiteInfo.FileNamespace + "{0}";
                SetClipboardText(format, selectedItems);
            }
        }

        public ICommand CopyWikilinkToClipboardCommand { get; }
        private void CopyWikilinkToClipboard(IList selectedItems)
        {
            IsChooseCopyTypePopupOpen = false;
            if (UploadedFiles.Count > 0)
            {
                string format = $"[[{_fileUploader.SiteInfo.FileNamespace}{{0}}]]";
                SetClipboardText(format, selectedItems);
            }
        }

        private void SetClipboardText(string entryFormat, IList selectedItems)
        {
            var text = MakeClipboardText(entryFormat, selectedItems);
            _helpers.SetClipboardText(text);
            IsCopiedPopupOpen = true;
        }

        private string MakeClipboardText(string format, IList selectedItems)
        {
            var files = selectedItems.Count == 0 ? UploadedFiles : selectedItems.Cast<UploadFile>();

            var fileNames = GetOrderedWikiFileNames(files);

            var output = new StringBuilder();
            foreach (var fileName in fileNames)
                output.AppendLine(string.Format(format, fileName));

            return output.ToString();
        }

        private IEnumerable<string> GetOrderedWikiFileNames(IEnumerable<UploadFile> files)
        {
            var fileNames = GetWikiFilenames(files);
            return SortWikiFileNames(fileNames);
        }

        private IEnumerable<string> SortWikiFileNames(IEnumerable<string> fileNames)
        {
            switch (SortingOption)
            {
                case SortingOptions.Ascending:
                    return fileNames.OrderBy(x => x);
                case SortingOptions.Descending:
                    return fileNames.OrderByDescending(x => x);
                default:
                    return fileNames;
            }
        }

        private IEnumerable<string> GetWikiFilenames(IEnumerable<UploadFile> files)
            => files.Select(x => _fileUploader.ServerFilename(x.FileName));

        public ICommand SortOrderCommand { get; }
        private void SortOrder(SortingOptions sortOption)
        {
            if (sortOption == SortingOption)
                return;

            IsUnsortedFocused = IsAcsendingFocused = IsDescendingFocused = false;
            UploadedFilesView.SortDescriptions.Clear();
            SortingOption = sortOption;
            switch (sortOption)
            {
                case SortingOptions.None:
                    IsUnsortedFocused = true;
                    break;
                case SortingOptions.Ascending:
                    SetOrder(ListSortDirection.Ascending);
                    IsAcsendingFocused = true;
                    break;
                case SortingOptions.Descending:
                    SetOrder(ListSortDirection.Descending);
                    IsDescendingFocused = true;
                    break;
            }
        }

         private void SetOrder(ListSortDirection listSortDirection)
            => UploadedFilesView.SortDescriptions.Add(CreateSortDescription(listSortDirection));

        private SortDescription CreateSortDescription(ListSortDirection listSortDirection)
            => new SortDescription(nameof(UploadFile.UploadFileName), listSortDirection);
    }
}
 