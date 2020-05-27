﻿using FluentFTP;
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

        public string DownloadFile(string path, string name, bool overwrightIfExists = true)
        {
            var path2 = path.Replace("/", @"\").Replace(@"\.", "").Substring(1);
            path2 = Path.Combine("TimeOnline", path2.Substring(0, path2.LastIndexOf(@"\")));
            var tmpPath = Path.Combine(Path.GetTempPath(), path2);
            if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
            var xpath = Path.Combine(tmpPath, name);
            if (!overwrightIfExists && File.Exists(xpath)) return xpath;
            var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
            using (var clientConnect2 = new FtpClient())
            {
                clientConnect2.Host = RGHSett.FtpHost;
                clientConnect2.Connect();
                var cl = clientConnect2.DownloadFile(xpath, path);
            }
            return xpath;
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
                Console.WriteLine(ex);
                throw;
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

                var path2 = remotePath.Replace("/", @"\").Replace(@"\.", "").Substring(1);
                path2 = Path.Combine("TimeOnline", path2);
                var tmpPath = Path.Combine(Path.GetTempPath(), path2);
                if (File.Exists(tmpPath)) File.Delete(tmpPath);
            }
            catch (Exception ex)
            {
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