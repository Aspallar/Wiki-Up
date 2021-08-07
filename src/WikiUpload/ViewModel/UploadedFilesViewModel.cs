using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;

namespace WikiUpload
{
    internal class UploadedFilesViewModel : BaseViewModel
    {
        private readonly IFileUploader _fileUploader;
        private readonly IHelpers _helpers;

        public UploadedFilesViewModel(IFileUploader fileUploader, IHelpers helpers)
        {
            _fileUploader = fileUploader;
            _helpers = helpers;

            UploadedFileSelectedIndex = -1;
            UploadedFiles = new UploadList(_helpers);
            UploadedFilesView = CollectionViewSource.GetDefaultView(UploadedFiles);
            LaunchFilePageCommand = new RelayParameterizedCommand((file) => LaunchFilePage((UploadFile)file));
            CopyToClipboardCommand = new RelayCommand(CopyToClipboard);
            SortOrderCommand = new RelayParameterizedCommand((sort) => SortOrder((SortingOptions)sort));
            RemoveFilesCommand = new RelayParameterizedCommand(RemoveFiles);
            RemoveSelectedFilesCommand = new RelayParameterizedCommand((selectedItems) => RemoveSelectedFiles((IList)selectedItems));
            ClearSelectionCommand = new RelayCommand(() => UploadedFileSelectedIndex = -1);
            RemoveAllFilesCommand = new RelayCommand(RemoveAllFiles);
        }

        public int UploadedFileSelectedIndex { get; set; }

        public UploadList UploadedFiles { get; }

        public ICollectionView UploadedFilesView { get; }

        public SortingOptions SortingOption { get; set; }

        public bool IsConfirmRemoveAllPopupOpen { get; set; }

        public ICommand ClearSelectionCommand { get; }

        public ICommand LaunchFilePageCommand { get; }

        private void LaunchFilePage(UploadFile file)
        {
            var url = _fileUploader.FileUrl(file.FileName);
            _helpers.LaunchProcess(url);
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
            var fileNames = GetOrderedWikiFileNames();

            var output = new StringBuilder();
            foreach (var fileName in fileNames)
                output.AppendLine(fileName);
            _helpers.SetClipboardText(output.ToString());
        }

        private IEnumerable<string> GetOrderedWikiFileNames()
        {
            var fileNames = GetWikiFilenames();
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

        private IEnumerable<string> GetWikiFilenames()
            => UploadedFiles.Select(x => _fileUploader.ServerFilename(x.FileName));

        public ICommand SortOrderCommand { get; }
        private void SortOrder(SortingOptions sortOption)
        {
            if (sortOption == SortingOption)
                return;

            UploadedFilesView.SortDescriptions.Clear();
            SortingOption = sortOption;
            switch (sortOption)
            {
                case SortingOptions.Ascending:
                    SetOrder(ListSortDirection.Ascending);
                    break;
                case SortingOptions.Descending:
                    SetOrder(ListSortDirection.Descending);
                    break;
            }
        }

        private void SetOrder(ListSortDirection listSortDirection)
            => UploadedFilesView.SortDescriptions.Add(CreateSortDescription(listSortDirection));

        private SortDescription CreateSortDescription(ListSortDirection listSortDirection)
            => new SortDescription(nameof(UploadFile.FileName), listSortDirection);
    }
}
 