using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace WikiUpload
{
    public class UploadViewModel : BaseViewModel, IFileDropTarget
    {
        private readonly DialogManager _dialogs;
        private CancellationTokenSource _cancelSource;

        public UploadViewModel()
        {
            _dialogs = new DialogManager();
            UploadSummary = "";
            PageContent = "";
            AddFilesCommand = new RelayCommand(() => AddFiles());
            RemoveFilesCommand = new RelayParameterizedCommand((selectedItems) => RemoveFiles(selectedItems));
            UploadCommand = new RelayCommand(async () => await Upload());
            CancelCommand = new RelayCommand(() => Cancel());
            LoadContentCommand = new RelayCommand(() => LoadContent());
            SaveContentCommand = new RelayCommand(() => SaveContent());
            LaunchSiteCommand = new RelayCommand(() => Process.Start(Site));
            LoadListCommand = new RelayCommand(() => LoadList());
            SaveListCommand = new RelayCommand(() => SaveList());
            ShowFileCommand = new RelayParameterizedCommand((filePath) => ShowImage((string)filePath));
            Site = UploadService.Uploader.Site;
        }

        private async Task Upload()
        {
            await RunCommand(() => this.UploadIsRunning, async () =>
            {
                using (_cancelSource = new CancellationTokenSource())
                {
                    CancellationToken cancelToken = _cancelSource.Token;
                    var filesToUpload = UploadFiles.Select(x => x).ToList();
                    UploadService.Uploader.Summary = AddAppName(UploadSummary);
                    UploadService.Uploader.PageContent = PageContent;
                    foreach (var file in filesToUpload)
                    {
                        if (!UploadService.Uploader.PermittedFiles.IsPermitted(file.FileName))
                        {
                            file.SetError($"Files of type \"{Path.GetExtension(file.FileName)}\" are not permitted.");
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
                                    file.SetError("Error reading file during upload.");
                                else
                                    file.SetError("Network error. Unable to upload.");
                            }
                            catch (XmlException)
                            {
                                file.SetError("Server returned an invalid XML response.");
                            }
                            catch (FileNotFoundException)
                            {
                                file.SetError($"File not found.");
                            }
                            catch (IOException)
                            {
                                file.SetError("Unable to read file.");
                            }
                            catch (TaskCanceledException)
                            {
                                if (cancelToken.IsCancellationRequested)
                                {
                                    file.SetError("Upload cancelled.");
                                    break; // foreach
                                }
                                else
                                {
                                    file.SetError("The upload timed out.");
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                file.SetError("Upload cancelled.");
                                break; // foreach
                            }
                            catch (ServerIsBusyException)
                            {
                                file.SetError("Server is too busy. Uploads cancelled. Try again later.");
                                break; // foreach
                            }
                            catch (NoEditTokenException)
                            {
                                file.SetError("Unable to obtain valid edit token. Uploads cancelled. You may have to restart Wiki-Up to resolve this error.");
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

                var response = await UploadService.Uploader.UpLoadAsync(file.FullPath, cancelToken, ForceUpload);
                await Task.Delay(Properties.Settings.Default.UploadDelay, cancelToken);

                if (response.Result == ResponseCodes.Success)
                {
                    UploadFiles.Remove(file);
                }
                else if (response.Result == ResponseCodes.Warning)
                {
                    file.SetWarning(response.WarningsText);
                }
                else if (response.Result == ResponseCodes.MaxlagThrottle)
                {
                    if (--maxLagRetries < 0)
                        throw new ServerIsBusyException();
                    await Task.Delay(response.RetryDelay * 1000, cancelToken);
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
                            await UploadService.Uploader.RefreshTokenAsync();
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
                    file.SetError("Unexpected server response");
                }
                return;
            }
        }

        private string AddAppName(string uploadSummary)
        {
            var appName = UploadService.Uploader.Site.ToLowerInvariant().Contains(".fandom.")
                ? "[[w:c:dev:Wiki-Up|Wiki-Up]]" : "Wiki-Up";

            return uploadSummary == ""
                ? $"Uploaded via {appName} {Utils.ApplicationVersion}"
                : $"{uploadSummary} (via {appName} {Utils.ApplicationVersion})";
        }

        private void RemoveFiles(object selectedItems)
        {
            // must guard against running upload because a delete keypress can fire this
            // as well as a button press.
            if (!UploadIsRunning)
                UploadFiles.RemoveRange(((IList)selectedItems).OfType<UploadFile>().ToList());
        }

        private void AddFiles()
        {
            if (_dialogs.AddFilesDialog(UploadService.Uploader.PermittedFiles.GetExtensions(),
                Properties.Settings.Default.ImageExtensions,
                out IList<string> fileNames))
            {
                UploadFiles.AddNewRange(fileNames);
            }
        }

        private void ShowImage(string fullPath)
        {
            try
            {
                Process.Start(fullPath);
            }
            catch (Win32Exception ex)
            {
                _dialogs.ErrorMessage(ex.Message);
            }
        }

        private void Cancel()
        {
            _cancelSource.Cancel();
        }

        private void LoadContent()
        {
            if (_dialogs.LoadContentDialog(out string fileName))
            {
                try
                {
                    PageContent = File.ReadAllText(fileName);
                }
                catch (IOException ex)
                {
                    _dialogs.ErrorMessage("Unable to read file. " + ex.Message);
                }
            }
        }

        private void SaveContent()
        {
            if (_dialogs.SaveContentDialog(out string fileName))
            {
                try
                {
                    File.WriteAllText(fileName, PageContent);
                }
                catch (IOException ex)
                {
                    _dialogs.ErrorMessage("Unable to save file. " + ex.Message);
                }
            }
        }

        private void LoadList()
        {
            if (_dialogs.LoadUploadListDialog(out string fileName))
            {
                try
                {
                    UploadFiles.AddFromXml(fileName);
                }
                catch (IOException ex)
                {
                    _dialogs.ErrorMessage("Unable to read upload list. " + ex.Message);
                }
                catch (InvalidOperationException)
                {
                    _dialogs.ErrorMessage("Invalid file format.");
                }
            }
        }

        private void SaveList()
        {
            if (_dialogs.SaveUploadListDialog(out string fileName))
            {
                try
                {
                    UploadFiles.SaveToXml(fileName);
                }
                catch (IOException ex)
                {
                    _dialogs.ErrorMessage("Unable to save file. " + ex.Message);
                }
            }
        }

        public void OnFileDrop(string[] filepaths)
        {
            if (!UploadIsRunning)
                UploadFiles.AddNewRange(filepaths);
        }

        public string Site { get; set; }

        public bool ForceUpload { get; set; }

        public string UploadSummary { get; set; }

        public string PageContent { get; set; }

        public ICommand AddFilesCommand { get; set; }

        public ICommand RemoveFilesCommand { get; set; }

        public ICommand UploadCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public ICommand LoadContentCommand { get; set; }

        public ICommand SaveContentCommand { get; set; }

        public ICommand LaunchSiteCommand { get; set; }

        public ICommand LoadListCommand { get; set; }

        public ICommand SaveListCommand { get; set; }

        public ICommand ShowFileCommand { get; set; }

        public bool UploadIsRunning { get; set; }

        public UploadList UploadFiles { get; set; } = new UploadList();

        public UploadFile ViewedFile { get; set; }
    }
}
