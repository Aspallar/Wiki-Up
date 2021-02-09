using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using WikiUpload.Properties;

namespace WikiUpload
{
    public class UploadViewModel : BaseViewModel, IFileDropTarget
    {
        private CancellationTokenSource _cancelSource;
        private IWikiSearch _categorySearch;
        private IWikiSearch _templateSearch;

        #region Constructor and dependencies

        private readonly IDialogManager _dialogs;
        private readonly IHelpers _helpers;
        private readonly INavigatorService _navigatorService;
        private readonly IUploadListSerializer _uploadFileSerializer;
        private readonly IWikiSearchFactory _wikiSearchFactory;
        private readonly IFileUploader _fileUploader;
        private readonly IAppSettings _appSettings;

        public UploadViewModel(IFileUploader fileUploader,
            IDialogManager dialogManager,
            IHelpers helpers,
            IUploadListSerializer uploadFileSerializer,
            INavigatorService navigatorService,
            IWikiSearchFactory wikiSearchFactory,
            IAppSettings appSettings)
        {
            _fileUploader = fileUploader;
            _appSettings = appSettings;
            _dialogs = dialogManager;
            _helpers = helpers;
            _navigatorService = navigatorService;
            _uploadFileSerializer = uploadFileSerializer;
            _wikiSearchFactory = wikiSearchFactory;

            ResetViewModel();

            LaunchSiteCommand = new RelayCommand(() => _helpers.LaunchProcess(_fileUploader.HomePage));
            ShowFileCommand = new RelayParameterizedCommand((filePath) => ShowImage((string)filePath));
            SignOutCommand = new RelayCommand(SignOut);

            // Mamage upload file list commands
            AddFilesCommand = new RelayCommand(AddFiles);
            RemoveFilesCommand = new RelayParameterizedCommand(RemoveFiles);
            LoadListCommand = new RelayCommand(LoadList);
            SaveListCommand = new RelayCommand(SaveList);

            // Upload commands
            UploadCommand = new RelayCommand(async () => await Upload());
            CancelCommand = new RelayCommand(Cancel);

            // Manage content commands
            LoadContentCommand = new RelayCommand(LoadContent);
            SaveContentCommand = new RelayCommand(SaveContent);

            // Catgory and Template commands (UploadPage)
            PickCategoryCommand = new RelayCommand(PickCategory);
            PickTemplateCommand = new RelayCommand(PickTemplate);

            // Catgory and Template commands (SearchPage)
            CancelSearchCommand = new RelayCommand(() => _navigatorService.NavigateToUploadPage());
            AddSearchItemCommand = new RelayParameterizedCommand(AddSearchItem);
            NextSearchCommand = new RelayCommand(async () => await NextSearch());
            StartSearchCommand = new RelayParameterizedCommand (async (from) => await StartSearch((string)from));
            PreviousSearchCommand = new RelayCommand(async () => await PreviousSearch());
        }
        #endregion

        #region Public Properties

        public bool ForceUpload { get; set; }
        public string UploadSummary { get; set; }
        public string PageContent { get; set; }
        public bool UploadIsRunning { get; set; }
        public bool SearchFetchInProgress { get; set; }
        public IWikiSearch CurrentSearch { get; set; }
        public UploadList UploadFiles { get; } = new UploadList();
        public UploadFile ViewedFile { get; set; }
        public bool IncludeInWatchlist { get; set; }

        public string Site
        {
            get
            {
                if (!string.IsNullOrEmpty(_fileUploader.ScriptPath)
                        && _fileUploader.Site.EndsWith(_fileUploader.ScriptPath))
                    return _fileUploader.Site.Substring(0, _fileUploader.Site.Length - _fileUploader.ScriptPath.Length);
                else
                    return _fileUploader.Site;
            }
        }

        #endregion

        #region Upload

        public ICommand UploadCommand { get; }
        private async Task Upload()
        {
            await RunCommand(() => UploadIsRunning, async () =>
            {
                using (_cancelSource = new CancellationTokenSource())
                {
                    CancellationToken cancelToken = _cancelSource.Token;
                    var filesToUpload = UploadFiles.Select(x => x).ToList();
                    _fileUploader.Summary = AddAppName(UploadSummary);
                    _fileUploader.PageContent = PageContent;
                    foreach (var file in filesToUpload)
                    {
                        if (!_fileUploader.PermittedFiles.IsPermitted(file.FileName))
                        {
                            file.SetError(UploadMessages.FileTypeNotPermitted(Path.GetExtension(file.FileName)));
                        }
                        else
                        {
                            try
                            {
                                await UploadFile(file, cancelToken);
                            }
                            catch (HttpRequestException ex)
                            {
                                if (ex.InnerException is IOException)
                                    file.SetError(UploadMessages.ReadFail);
                                else
                                    file.SetError(UploadMessages.NetworkError);
                            }
                            catch (XmlException)
                            {
                                file.SetError(UploadMessages.InvalidXml);
                            }
                            catch (FileNotFoundException)
                            {
                                file.SetError(UploadMessages.FileNotFound);
                            }
                            catch (IOException)
                            {
                                file.SetError(UploadMessages.ReadFail);
                            }
                            catch (TaskCanceledException)
                            {
                                if (_helpers.IsCancellationRequested(cancelToken))
                                {
                                    file.SetError(UploadMessages.Cancelled);
                                    break; // foreach
                                }
                                else
                                {
                                    file.SetError(UploadMessages.TimedOut);
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                file.SetError(UploadMessages.Cancelled);
                                break; // foreach
                            }
                            catch (ServerIsBusyException)
                            {
                                file.SetError(UploadMessages.ServerBusy);
                                break; // foreach
                            }
                            catch (NoEditTokenException)
                            {
                                file.SetError(UploadMessages.NoEditToken);
                                break; // foreach
                            }
                        }
                    }
                }
            });
        }

        private async Task UploadFile(UploadFile file, CancellationToken cancelToken)
        {
            int maxLagRetries = 3;
            bool tokenRefreshed = false;
            while (true)
            {
                cancelToken.ThrowIfCancellationRequested();

                file.SetUploading();
                ViewedFile = file;

                var response = await _fileUploader.UpLoadAsync(file.FullPath, cancelToken, ForceUpload, IncludeInWatchlist);
                await _helpers.Wait(_appSettings.UploadDelay, cancelToken);

                // Note: Only access response.Result once, as thgis makes testing much easier
                //       as a chain of responses can be faked. See maclag tests in UploadViewModelTests.cs
                var result = response.Result;

                if (result == ResponseCodes.Success)
                {
                    UploadFiles.Remove(file);
                }
                else if (result == ResponseCodes.Warning)
                {
                    file.SetWarning(response.WarningsText);
                }
                else if (result == ResponseCodes.MaxlagThrottle)
                {
                    if (--maxLagRetries < 0)
                        throw new ServerIsBusyException();
                    await _helpers.Wait(response.RetryDelay * 1000, cancelToken);
                    continue;
                }
                else if (response.IsError)
                {
                    if (response.IsTokenError)
                    {
                        if (tokenRefreshed)
                        {
                            throw new NoEditTokenException();
                        }
                        else
                        {
                            await _fileUploader.RefreshTokenAsync();
                            tokenRefreshed = true;
                            continue;
                        }
                    }
                    else
                    {
                        file.SetError(response.ErrorsText);
                    }
                }
                else
                {
                    file.SetError(UploadMessages.UnkownServerResponse);
                }
                return;
            }
        }

        private string AddAppName(string uploadSummary)
        {
            var appName = Site.ToLowerInvariant().Contains(".fandom.")
                ? "[[w:c:dev:Wiki-Up|Wiki-Up]]" : "Wiki-Up";

            return uploadSummary == ""
                ? $"{Resources.UploadViaText} {appName} {_helpers.ApplicationVersion}"
                : $"{uploadSummary} ({Resources.ViaText} {appName} {_helpers.ApplicationVersion})";
        }

        public ICommand CancelCommand { get; }
        private void Cancel()
        {
            _helpers.SignalCancel(_cancelSource);
        }

        #endregion

        #region Mamage upload file list

        public ICommand AddFilesCommand { get; }
        private void AddFiles()
        {
            if (_dialogs.AddFilesDialog(_fileUploader.PermittedFiles.GetExtensions(),
                _appSettings.ImageExtensions,
                out IList<string> fileNames))
            {
                UploadFiles.AddNewRange(fileNames);
            }
        }

        public ICommand RemoveFilesCommand { get; }
        private void RemoveFiles(object selectedItems)
        {
            // must guard against running upload because a delete keypress can fire this
            // as well as a button press.
            if (!UploadIsRunning)
                UploadFiles.RemoveRange(((IList)selectedItems).OfType<UploadFile>().ToList());
        }

        public ICommand LoadListCommand { get; }
        private void LoadList()
        {
            if (_dialogs.LoadUploadListDialog(out string fileName))
            {
                try
                {
                    _uploadFileSerializer.Add(fileName, UploadFiles);
                }
                catch (Exception ex)
                {
                    _dialogs.ErrorMessage(Resources.CantReadUploadListMessage, ex);
                }
            }
        }

        public ICommand SaveListCommand { get; }
        private void SaveList()
        {
            if (_dialogs.SaveUploadListDialog(out string fileName))
            {
                try
                {
                    _uploadFileSerializer.Save(fileName, UploadFiles);
                }
                catch (Exception ex)
                {
                    _dialogs.ErrorMessage(Resources.CantSaveUploadListMessage, ex);
                }
            }
        }

        public void OnFileDrop(string[] filepaths)
        {
            if (!UploadIsRunning)
                UploadFiles.AddNewRange(filepaths);
        }

        #endregion

        #region Manage content

        public ICommand LoadContentCommand { get; }
        private void LoadContent()
        {
            if (_dialogs.LoadContentDialog(out string fileName))
            {
                try
                {
                    PageContent = _helpers.ReadAllText(fileName);
                }
                catch (Exception ex)
                {
                    _dialogs.ErrorMessage(Resources.CantReadContentMessage, ex);
                }
            }
        }

        public ICommand SaveContentCommand { get; }
        private void SaveContent()
        {
            if (_dialogs.SaveContentDialog(out string fileName))
            {
                try
                {
                    _helpers.WriteAllText(fileName, PageContent);
                }
                catch (Exception ex)
                {
                    _dialogs.ErrorMessage(Resources.CantSaveContentMessage, ex);
                }
            }
        }
        #endregion

        #region Categories and Templates

        public ICommand CancelSearchCommand { get; }

        public ICommand PickCategoryCommand { get; }
        private void PickCategory()
        {
            CurrentSearch = _categorySearch;
            _navigatorService.NavigateToSearchPage();
        }

        public ICommand PickTemplateCommand { get; }
        private void PickTemplate()
        {
            CurrentSearch = _templateSearch;
            _navigatorService.NavigateToSearchPage();
        }

        public ICommand AddSearchItemCommand { get; }
        private void AddSearchItem(object item)
        {
            var itemString = (string)item;
            if (!string.IsNullOrWhiteSpace(itemString))
            {
                bool needNewline = PageContent != "" && !PageContent.EndsWith("\n");
                string newLine = needNewline ? "\n" : "";
                PageContent += newLine + CurrentSearch.FullItemString(itemString);
                _navigatorService.NavigateToUploadPage();
            }
        }

        public ICommand StartSearchCommand { get; }
        private async Task StartSearch(string from)
        {
            if (!SearchFetchInProgress)
            {
                SearchFetchInProgress = true;
                await CurrentSearch.Start(from);
                SearchFetchInProgress = false;
            }
        }

        public ICommand NextSearchCommand { get; }
        private async Task NextSearch()
        {
            if (CurrentSearch != null &&  !SearchFetchInProgress && CurrentSearch.HasNext)
            {
                SearchFetchInProgress = true;
                await CurrentSearch.Next();
                SearchFetchInProgress = false;
            }
        }

        public ICommand PreviousSearchCommand { get; }
        private async Task PreviousSearch()
        {
            if (CurrentSearch != null &&  !SearchFetchInProgress && CurrentSearch.HasPrevious)
            {
                SearchFetchInProgress = true;
                await CurrentSearch.Previous();
                SearchFetchInProgress = false;
            }
        }

        #endregion

        #region Launch

        public ICommand LaunchSiteCommand { get; }

        public ICommand ShowFileCommand { get; }
        private void ShowImage(string fullPath)
        {
            try
            {
                _helpers.LaunchProcess(fullPath);
            }
            catch (Win32Exception ex)
            {
                _dialogs.ErrorMessage(Resources.ViewImageErrorMessage, ex);
            }
        }

        #endregion

        #region Sign out

        public ICommand SignOutCommand { get; set; }
        private void SignOut()
        {
            _fileUploader.LogOff();
            ResetViewModel();
            _navigatorService.NewUploadPage();
            _navigatorService.NavigateToLoginPage();
        }

        private void ResetViewModel()
        {
            UploadSummary = "";
            PageContent = "";
            UploadFiles.Clear();
            _templateSearch = _wikiSearchFactory.CreateTemplateSearch(_fileUploader);
            _categorySearch = _wikiSearchFactory.CreateCategorySearch(_fileUploader);
        }

        #endregion

    }
}
