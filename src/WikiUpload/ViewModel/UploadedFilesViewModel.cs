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
        private IFileUploader _fileUploader;
        private IHelpers _helpers;

        public UploadedFilesViewModel(IFileUploader fileUploader, IHelpers helpers)
        {
            _fileUploader = fileUploader;
            _helpers = helpers;

            UploadedFileSeletedIndex = -1;
            UploadedFiles = new UploadList(_helpers);
            UploadedFilesView = CollectionViewSource.GetDefaultView(UploadedFiles);
            LaunchFilePageCommand = new RelayParameterizedCommand((file) => LaunchFilePage((UploadFile)file));
            CopyToClipboardCommand = new RelayCommand(CopyToClipboard);
            SortOrderCommand = new RelayParameterizedCommand((sort) => SortOrder((SortingOptions)sort));
            RemoveFilesCommand = new RelayParameterizedCommand(RemoveFiles);
            ClearSelectionCommand = new RelayCommand(() => UploadedFileSeletedIndex = -1);
        }

        public int UploadedFileSeletedIndex { get; set; }

        public UploadList UploadedFiles { get; }

        public ICollectionView UploadedFilesView { get; }

        public SortingOptions SortingOption { get; set; }

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
            if (((IList)selectedItems).Count == 0)
                UploadedFiles.Clear();
            else
                UploadedFiles.RemoveRange(((IList)selectedItems).OfType<UploadFile>().ToList());
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
 