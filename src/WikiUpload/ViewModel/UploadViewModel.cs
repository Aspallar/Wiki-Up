using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using WikiUpload.Properties;

namespace WikiUpload
{
    internal class UploadViewModel : BaseViewModel, IFileDropTarget
    {
        private CancellationTokenSource _cancelSource;
        private IWikiSearch _categorySearch;
        private IWikiSearch _templateSearch;
        private bool _editTokenRefreshed;

        #region Constructor and dependencies

        private readonly IDialogManager _dialogs;
        private readonly IHelpers _helpers;
        private readonly INavigatorService _navigatorService;
        private readonly IUploadListSerializer _uploadFileSerializer;
        private readonly IYoutube _youtube;
        private readonly IFileFinder _fileFinder;
        private readonly IWikiSearchFactory _wikiSearchFactory;
        private readonly IFileUploader _fileUploader;
        private readonly IAppSettings _appSettings;

        public UploadViewModel(IFileUploader fileUploader,
            IDialogManager dialogManager,
            IHelpers helpers,
            IUploadListSerializer uploadFileSerializer,
            INavigatorService navigatorService,
            IWikiSearchFactory wikiSearchFactory,
            IYoutube youtube,
            IFileFinder fileFinder,
            IAppSettings appSettings)
        {
            _fileUploader = fileUploader;
            _appSettings = appSettings;
            _dialogs = dialogManager;
            _helpers = helpers;
            _navigatorService = navigatorService;
            _uploadFileSerializer = uploadFileSerializer;
            _youtube = youtube;
            _fileFinder = fileFinder;
            _wikiSearchFactory = wikiSearchFactory;

            UploadFiles = new UploadList(_helpers);
            ResetViewModel();

            LaunchSiteCommand = new RelayCommand(() => _helpers.LaunchProcess(_fileUploader.HomePage));
            ShowFileCommand = new RelayParameterizedCommand((filePath) => ShowImage((string)filePath));
            SignOutCommand = new RelayCommand(SignOut);

            // Manage upload file list commands
            AddFilesCommand = new RelayCommand(AddFiles);
            AddFolderCommand = new RelayCommand(AddFolder);
            RemoveFilesCommand = new RelayParameterizedCommand(RemoveFiles);
            LoadListCommand = new RelayCommand(LoadList);
            SaveListCommand = new RelayCommand(SaveList);

            // Upload commands
            UploadCommand = new RelayCommand(async () => await Upload());
            CancelCommand = new RelayCommand(Cancel);

            // Manage content commands
            LoadContentCommand = new RelayCommand(LoadContent);
            SaveContentCommand = new RelayCommand(SaveContent);

            // Category and Template commands (UploadPage)
            PickCategoryCommand = new RelayCommand(PickCategory);
            PickTemplateCommand = new RelayCommand(PickTemplate);

            // Category and Template commands (SearchPage)
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
        public UploadList UploadFiles { get; }
        public UploadFile ViewedFile { get; set; }
        public bool IncludeInWatchlist { get; set; }
        public bool AddingFiles { get; set; } = false;

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
            if (AddingFiles)
                return;

            var needsLogin = false;
            await RunCommand(() => UploadIsRunning, async () =>
            {
                using (_cancelSource = new CancellationTokenSource())
                {
                    var cancelToken = _cancelSource.Token;
                    var filesToUpload = new List<UploadFile>(UploadFiles);
                    var variableSummary = new VariableContent(AddAppName(UploadSummary));
                    var variablePageContent = new VariablePageContent(_appSettings.ContentFileExtension, PageContent, _helpers);
                    _fileUploader.IncludeInWatchList = IncludeInWatchlist;
                    _fileUploader.IgnoreWarnings = ForceUpload;
                    _editTokenRefreshed = false;
                    foreach (var file in filesToUpload)
                    {
                        if (!file.IsVideo && !_fileUploader.PermittedFiles.IsPermitted(file.FileName))
                        {
                            file.SetError(UploadMessages.FileTypeNotPermitted(Path.GetExtension(file.FileName)));
                        }
                        else
                        {
                            SetViewdFile(file);
                            try
                            {
                                if (file.IsVideo)
                                    await UploadVideo(file, cancelToken);
                                else
                                    await UploadFile(
                                        file,
                                        variableSummary.ExpandedContent(file),
                                        variablePageContent.ExpandedContent(file),
                                        cancelToken);
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
                            catch (JsonException)
                            {
                                file.SetError(UploadMessages.InvalidJson);
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
                            catch (MustBeLoggedInException)
                            {
                                needsLogin = true;
                                break;
                            }
                        }
                    }
                }
            });
            if (needsLogin)
                LoginAgain();
        }

        private void SetViewdFile(UploadFile file)
        {
            if (_appSettings.FollowUploadFile)
                ViewedFile = file;
        }

        private async Task UploadVideo(UploadFile file, CancellationToken cancelToken)
        {
            file.SetUploading();
            while (true)
            {
                IngestionControllerResponse response;
                try
                {
                    response = await _fileUploader.UpLoadVideoAsync(file.FullPath, cancelToken);
                }
                finally
                {
                    await _helpers.Wait(_appSettings.UploadDelay, cancelToken);
                }
                if (response.Success)
                {
                    UploadFiles.Remove(file);
                    break; //while true
                }
                else
                {
                    if (_editTokenRefreshed || response.HttpStatusCode != HttpStatusCode.BadRequest)
                    {
                        file.SetError(response.Status);
                        break; // while true
                    }
                    else
                    {
                        await RefreshEditToken();
                    }
                }
            }
        }

        private async Task UploadFile(UploadFile file, string summary, string newPageContent, CancellationToken cancelToken)
        {
            var maxLagRetries = 3;
            while (true)
            {
                cancelToken.ThrowIfCancellationRequested();
    
                file.SetUploading();

                IUploadResponse response;
                try
                {
                    response = await _fileUploader.UpLoadAsync(
                        file.FullPath,
                        cancelToken,
                        summary,
                        newPageContent);
                }
                finally
                {
                    await _helpers.Wait(_appSettings.UploadDelay, cancelToken);
                }

                // Note: Only access response.Result once, as thgis makes testing much easier
                //       as a chain of responses can be faked. See maclag tests in UploadViewModelTests.cs
                var result = response.Result;

                if (result == ResponseCodes.Success)
                {
                    UploadFiles.Remove(file);
                }
                else if (result == ResponseCodes.Warning)
                {
                    file.SetWarning(response.Warnings.ToString());
                }
                else if (result == ResponseCodes.MaxlagThrottle)
                {
                    if (--maxLagRetries < 0)
                        throw new ServerIsBusyException();
                    await _helpers.Wait(response.RetryDelay * 1000, cancelToken);
                    continue;
                }
                else if (response.Errors.IsAny)
                {
                    if (response.Errors.IsTokenError)
                    {
                        if (_editTokenRefreshed)
                        {
                            throw new NoEditTokenException();
                        }
                        else
                        {
                            await RefreshEditToken();
                            continue;
                        }
                    }
                    else if (response.Errors.IsMutsBeLoggedInError)
                    {
                        file.SetError(response.Errors.ToString());
                        throw new MustBeLoggedInException();
                    }
                    else if (response.Errors.IsRateLimitedError)
                    {
                        file.SetDelaying(Resources.WaitingForRetry + " " + response.Errors.ToString());
                        await _helpers.Wait(_appSettings.RateLimitedBackoffPeriod, cancelToken);
                        continue;
                    }
                    else
                    {
                        file.SetError(response.Errors.ToString());
                    }
                }
                else
                {
                    file.SetError(UploadMessages.UnkownServerResponse);
                }
                return;
            }
        }

        private async Task RefreshEditToken()
        {
            await _fileUploader.RefreshTokenAsync();
            _editTokenRefreshed = true;
        }

        private string AddAppName(string uploadSummary)
        {
            var appName = Site.ToLowerInvariant().Contains(".fandom.")
                ? "[[w:c:dev:Wiki-Up|Wiki-Up]]" : "Wiki-Up";

            return uploadSummary == ""
                ? $"{Resources.UploadViaText} {appName} {_helpers.ApplicationVersionString}"
                : $"{uploadSummary} ({Resources.ViaText} {appName} {_helpers.ApplicationVersionString})";
        }

        public ICommand CancelCommand { get; }
        private void Cancel()
        {
            _helpers.SignalCancel(_cancelSource);
        }

        #endregion

        #region Manage upload file list

        public ICommand AddFilesCommand { get; }
        private async void AddFiles()
        {
            if (AddingFiles)
                return;

            if (_dialogs.AddFilesDialog(_fileUploader.PermittedFiles.GetExtensions(),
                _appSettings.ImageExtensions,
                out var fileNames))
            {
                AddingFiles = true;
                await UploadFiles.AddNewRangeAsync(fileNames);
                AddingFiles = false;
            }
        }

        public ICommand AddFolderCommand { get; }
        private async void AddFolder()
        {
            if (AddingFiles)
                return;

            if (_dialogs.AddFolderDialog(out var folderPath))
            {
                if (_dialogs.AddFolderOptionsDialog(folderPath,
                    out var includeSunfolder,
                    out var includeFiles,
                    out var extension))
                {
                    try
                    {
                        AddingFiles = true;
                        var fileNames = _fileFinder.GetFiles(folderPath, includeSunfolder, includeFiles, extension);
                        await UploadFiles.AddNewRangeAsync(fileNames);
                    }
                    catch (Exception ex)
                    {
                        _dialogs.ErrorMessage(Resources.UnableToAddFiles, ex);
                    }
                    finally
                    {
                        AddingFiles = false;
                    }
                }
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
        private async void LoadList()
        {
            if (AddingFiles)
                return;

            if (_dialogs.LoadUploadListDialog(out var fileName))
            {
                AddingFiles = true;
                try
                {
                    var newFiles = _uploadFileSerializer.Deserialize(fileName);
                    await UploadFiles.AddRangeAsync(newFiles);
                }
                catch (Exception ex)
                {
                    _dialogs.ErrorMessage(Resources.CantReadUploadListMessage, ex);
                }
                finally
                {
                    AddingFiles = false;
                }
            }
        }

        public ICommand SaveListCommand { get; }
        private void SaveList()
        {
            if (_dialogs.SaveUploadListDialog(out var fileName))
            {
                try
                {
                    _uploadFileSerializer.Serialize(fileName, UploadFiles);
                }
                catch (Exception ex)
                {
                    _dialogs.ErrorMessage(Resources.CantSaveUploadListMessage, ex);
                }
            }
        }

        #endregion

        #region File drag drop

        public async void OnFileDrop(string[] filepaths, bool controlKeyPressed)
        {
            // TODO: OnFileDrop fix dropping folders being allowed
            if (!UploadIsRunning && !AddingFiles)
            {
                AddingFiles = true;
                if (filepaths.Length == 1 && !controlKeyPressed)
                {
                    var youtubePlaylistId = _youtube.ExtractPlaylistId(filepaths[0]);
                    if (youtubePlaylistId != null)
                        await AddYoutubePlaylistVideos(youtubePlaylistId);
                    else
                        UploadFiles.AddNewRange(filepaths);
                }
                else
                {
                    await UploadFiles.AddNewRangeAsync(filepaths);
                }
                AddingFiles = false;
            }
        }

        private async Task AddYoutubePlaylistVideos(string youtubePlaylistId)
        {
            const int maxPlayllistLength = 200;
            try
            {
                var videoLinks = await _youtube.FetchPlasylistViedeoLinksAsync(youtubePlaylistId, maxPlayllistLength);
                UploadFiles.AddNewRange(videoLinks.ToList());
            }
            catch (TooManyVideosException)
            {
                _dialogs.ErrorMessage(Resources.PlalistTooBig,
                    string.Format(Resources.PlalistMaximumLength, maxPlayllistLength));
            }
            catch (Exception)
            {
                _dialogs.ErrorMessage(Resources.YoutubeError);
            }
        }

        #endregion

        #region Manage content

        public ICommand LoadContentCommand { get; }
        private void LoadContent()
        {
            if (_dialogs.LoadContentDialog(out var fileName))
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
            if (_dialogs.SaveContentDialog(out var fileName))
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
                var needNewline = PageContent != "" && !PageContent.EndsWith("\n");
                var newLine = needNewline ? "\n" : "";
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

        public void LoginAgain()
        {
            if(_dialogs.ErrorMessage(Resources.LoginExpiredText, Resources.LoginExpiredSubtext, true))
            {
                _fileUploader.LogOff();
                _navigatorService.NewUploadPage();
                _navigatorService.NavigateToLoginPage();
            }
        }

        #endregion

    }
}
 