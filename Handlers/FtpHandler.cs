using FluentFTP;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace RetroGameHandler.Handlers
{
    public static class FtpHandler
    {
        public static FtpHelper Instance { get; } = new FtpHelper();
    }

    public class FtpHelper : IDisposable, INotifyPropertyChanged
    {
        // Instantiate a SafeHandle instance.
        private readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        private string _eAT;

        private int _fileCount;

        private int _fileIndex;
        private long _transferredBytes;

        public long TransferredBytes
        {
            get => _transferredBytes;
            set
            {
                _transferredBytes = value;
                NotifyPropertyChanged();
            }
        }

        private FtpOptions _options = new FtpOptions();

        private double _progress = 0;

        private double _transferSpeed;
        private string _transMessage;
        private string _transferSpeedString;

        private bool disposed = false;

        public FtpHelper()
        {
            UppdateConnectionStatus();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string EAT
        {
            get => _eAT;
            set
            {
                _eAT = value;
                NotifyPropertyChanged();
            }
        }

        public string TransferSpeedString
        {
            get => _transferSpeedString;
            set
            {
                _transferSpeedString = value;
                NotifyPropertyChanged();
            }
        }

        public string TransMessage
        {
            get => _transMessage;
            set
            {
                _transMessage = value;
                NotifyPropertyChanged();
            }
        }

        public int FileCount
        {
            get => _fileCount;
            set
            {
                _fileCount = value;
                NotifyPropertyChanged();
            }
        }

        public int FileIndex
        {
            get => _fileIndex;
            set
            {
                _fileIndex = value;
                NotifyPropertyChanged();
            }
        }

        public FtpOptions FTPOptions
        {
            get => _options; set
            {
                _options = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsConnected { get => clientConnect?.IsConnected ?? false; }

        public double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                NotifyPropertyChanged();
            }
        }

        public double TransferSpeed
        {
            get => _transferSpeed;
            set
            {
                _transferSpeed = value;
                NotifyPropertyChanged();
            }
        }

        private FtpClient clientConnect { get; set; }

        public void Dispose()
        {
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        public async Task<IList<FtpResult>> DownloadDirectoryAsync(string remotePath, string localPath, CancellationToken cancellationToken, Progress<FtpProgress> progress, FtpFolderSyncMode SyncMode = FtpFolderSyncMode.Update)
        {
            try
            {
                //if (!overwrightIfExists && File.Exists(localPath)) return new List<FtpResult>();
                var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                using (var clientConnect2 = new FtpClient())
                {
                    clientConnect2.Host = RGHSett.FtpHost;
                    clientConnect2.Connect();
                    var cl = await clientConnect2.DownloadDirectoryAsync(localPath, remotePath, SyncMode, FtpLocalExists.Append, FtpVerify.Retry, null, progress, cancellationToken);
                    return cl;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                return new List<FtpResult>();
            }
        }

        public async Task<FtpStatus> DownloadFileAsync(string remotePath, string localPath, CancellationToken cancellationToken, Progress<FtpProgress> progress)
        {
            try
            {
                //if (!overwrightIfExists && File.Exists(localPath)) return new List<FtpResult>();
                var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                using (var clientConnect2 = new FtpClient())
                {
                    clientConnect2.Host = RGHSett.FtpHost;
                    clientConnect2.Connect();
                    var cl = await clientConnect2.DownloadFileAsync(localPath, remotePath, FtpLocalExists.Append, FtpVerify.Retry, progress, cancellationToken);
                    return cl;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                return FtpStatus.Failed;
            }
        }

        public string DownloadFile(string remotePath, string localPath, bool overwrightIfExists = true)
        {
            try
            {
                if (!overwrightIfExists && File.Exists(localPath)) return localPath;
                var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                using (var clientConnect2 = new FtpClient())
                {
                    clientConnect2.Host = RGHSett.FtpHost;
                    clientConnect2.Connect();
                    var cl = clientConnect2.DownloadFile(localPath, remotePath);
                }
                return localPath;
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                return localPath;
            }
        }

        public bool DownloadStream(string path, Stream outStream)
        {
            try
            {
                var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                using (var clientConnect2 = new FtpClient())
                {
                    clientConnect2.Host = RGHSett.FtpHost;
                    clientConnect2.Connect();
                    return clientConnect2.Download(outStream, path);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> UploadStreamAsync(Stream data, string filePathName)
        {
            try
            {
                var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                using (var clientConnect2 = new FtpClient())
                {
                    clientConnect2.Host = RGHSett.FtpHost;
                    clientConnect2.Connect();
                    Progress<FtpProgress> progress = new Progress<FtpProgress>((p) =>
                    {
                        TransMessage = $"Upload to {p.RemotePath}";
                        Progress = p.Progress;
                        FileCount = p.FileCount;
                        FileIndex = p.FileIndex;
                        TransferSpeed = p.TransferSpeed;
                        //EAT = p.ETA.ToString("hh:mm");
                        TransferredBytes = p.TransferredBytes;
                    });
                    var status = await clientConnect2.UploadAsync(data, filePathName, FtpRemoteExists.Overwrite, true, progress);
                    return status.IsSuccess();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                Console.WriteLine(ex);
                return false;
            }
        }

        public void UploadFile(string[] localPath, string remotePath)
        {
            try
            {
                Action<FtpProgress> progress = delegate (FtpProgress p)
                {
                    if (p.Progress == 1)
                    {
                        Progress = 100;
                    }
                    else
                    {
                        TransMessage = $"Upload from {p.LocalPath} to {p.RemotePath}";
                        Progress = p.Progress;
                        FileCount = p.FileCount;
                        FileIndex = p.FileIndex;
                        TransferSpeed = p.TransferSpeed;
                        EAT = p.ETA.ToString("hh:mm");
                        TransferredBytes = p.TransferredBytes;
                    }
                };
                var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                using (var clientConnect2 = new FtpClient())
                {
                    clientConnect2.Host = RGHSett.FtpHost;
                    clientConnect2.Connect();
                    var cl = clientConnect2.UploadFiles(localPath, remotePath, progress: progress);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UploadFilesAsync(string[] localPath, string remotePath, Progress<FtpProgress> progress, CancellationToken cancellationToken)
        {
            try
            {
                var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                using (var clientConnect2 = new FtpClient())
                {
                    clientConnect2.Host = RGHSett.FtpHost;
                    await clientConnect2.AutoConnectAsync();
                    await clientConnect2.UploadFilesAsync(localPath, remotePath, token: cancellationToken, progress: progress);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<bool> FieExist(string Filepath)
        {
            try
            {
                var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                using (var clientConnect2 = new FtpClient())
                {
                    clientConnect2.Host = RGHSett.FtpHost;
                    await clientConnect2.AutoConnectAsync();
                    return await clientConnect2.FileExistsAsync(Filepath);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task DeletedFileAsync(string remotePath, CancellationToken cancellationToken)
        {
            try
            {
                var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                using (var clientConnect2 = new FtpClient())
                {
                    clientConnect2.Host = RGHSett.FtpHost;
                    await clientConnect2.AutoConnectAsync();
                    await clientConnect2.DeleteFileAsync(remotePath, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task RenameDirectory(string fromPath, string toPath, CancellationToken cancellationToken)
        {
            try
            {
                var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                using (var clientConnect2 = new FtpClient())
                {
                    clientConnect2.Host = RGHSett.FtpHost;
                    await clientConnect2.AutoConnectAsync();
                    await clientConnect2.MoveDirectoryAsync(fromPath, toPath, FtpRemoteExists.Overwrite, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task RenameFile(string fromPath, string toPath, CancellationToken cancellationToken)
        {
            try
            {
                var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                using (var clientConnect2 = new FtpClient())
                {
                    clientConnect2.Host = RGHSett.FtpHost;
                    await clientConnect2.AutoConnectAsync();
                    await clientConnect2.MoveFileAsync(fromPath, toPath, FtpRemoteExists.Overwrite, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task DeleteDirectoryAsync(string remotePath, CancellationToken cancellationToken)
        {
            try
            {
                var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                using (var clientConnect2 = new FtpClient())
                {
                    clientConnect2.Host = RGHSett.FtpHost;
                    await clientConnect2.AutoConnectAsync();
                    await clientConnect2.DeleteDirectoryAsync(remotePath, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                Console.WriteLine(ex);
                throw;
            }
        }

        public Task<bool> IsValidConnection()
        {
            return Task.Run<bool>(() =>
            {
                try
                {
                    var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                    using (var clientConnect2 = new FtpClient())
                    {
                        clientConnect2.ConnectTimeout = 5000;
                        clientConnect2.Host = RGHSett.FtpHost;
                        clientConnect2.Connect();
                        return clientConnect2.IsConnected;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            });
        }

        public IEnumerable<FtpListItem> List(string path, FtpListOption ftpListOption = FtpListOption.AllFiles)
        {
            try
            {
                var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                FtpListItem[] list;
                using (var clientConnect2 = new FtpClient())
                {
                    clientConnect2.Host = RGHSett.FtpHost;
                    clientConnect2.Connect();
                    list = clientConnect2.GetListing(path, ftpListOption);
                }
                return list;
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                Console.WriteLine(ex);
                return new List<FtpListItem>();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                handle.Dispose();
                if (!clientConnect.IsDisposed) clientConnect.Dispose();
            }
            disposed = true;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnProgress(FtpProgress ftpProgress)
        {
            if (ftpProgress.Progress == 1)
            {
                Progress = 100;
            }
            else
            {
                Progress = (ftpProgress.Progress * 100);
            }
            FileCount = ftpProgress.FileCount;
            FileIndex = ftpProgress.FileIndex;
            TransferSpeed = ftpProgress.TransferSpeed;
            EAT = ftpProgress.ETA.ToString("hh:mm");
        }

        private void UppdateConnectionStatus()
        {
            var timer = new System.Timers.Timer(2000);
            timer.Elapsed += (s, e) => NotifyPropertyChanged("IsConnected");
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        [Serializable]
        public class FtpOptions : INotifyPropertyChanged
        {
            private bool _createRemoteDir = true;
            private FtpError _ftpError = FtpError.DeleteProcessed;
            private FtpFolderSyncMode _ftpFolderSyncMode;
            private FtpLocalExists _ftpLocalExists;
            private FtpRemoteExists _ftpRemoteExists = FtpRemoteExists.Skip;
            private FtpVerify _ftpVerify = FtpVerify.Retry;
            private string _host = "192.168.0.120"; //"10.1.1.2";
            private string _lastPath = "";
            private string _password = "";
            private bool _useCredantial = false;
            private string _userName = "";

            public event PropertyChangedEventHandler PropertyChanged;

            public bool CreateRemoteDir
            {
                get => _createRemoteDir; set
                {
                    _createRemoteDir = value;
                    NotifyPropertyChanged();
                }
            }

            public FtpError FtpError
            {
                get => _ftpError; set
                {
                    _ftpError = value;
                    NotifyPropertyChanged();
                }
            }

            public FtpFolderSyncMode FtpFolderSyncMode
            {
                get => _ftpFolderSyncMode;
                set
                {
                    _ftpFolderSyncMode = value;
                    NotifyPropertyChanged();
                }
            }

            public FtpLocalExists FtpLocalExists
            {
                get => _ftpLocalExists;
                set
                {
                    _ftpLocalExists = value;
                    NotifyPropertyChanged();
                }
            }

            public FtpRemoteExists FtpRemoteExists
            {
                get => _ftpRemoteExists;
                set
                {
                    _ftpRemoteExists = value;
                    NotifyPropertyChanged();
                }
            }

            public FtpVerify FtpVerify
            {
                get => _ftpVerify; set
                {
                    _ftpVerify = value;
                    NotifyPropertyChanged();
                }
            }

            public string Host
            {
                get => _host;
                set
                {
                    _host = value;
                    NotifyPropertyChanged();
                }
            }

            public string LastPath
            {
                get => _lastPath;
                set
                {
                    _lastPath = value;
                    NotifyPropertyChanged();
                }
            }

            public string Password
            {
                get => _password;
                set
                {
                    _password = value;
                    NotifyPropertyChanged();
                }
            }

            public bool UseCredantial
            {
                get => _useCredantial;
                set
                {
                    _useCredantial = value;
                    NotifyPropertyChanged();
                }
            }

            public string UserName
            {
                get => _userName;
                set
                {
                    _userName = value;
                    NotifyPropertyChanged();
                }
            }

            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}