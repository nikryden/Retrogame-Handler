using FluentFTP;
using Microsoft.Win32;
using RetroGameHandler.Handlers;
using RetroGameHandler.Helpers;
using RetroGameHandler.models;
using RetroGameHandler.ViewModels;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RetroGameHandler.Views
{
    /// <summary>
    /// Interaction logic for StartView.xaml
    /// </summary>
    public partial class StartView : UserControl, IPage
    {
        public StartView()
        {
            InitializeComponent();
            this.DataContext = ViewModel;
            DirFileInfo.Visibility = Visibility.Hidden;
            BatteryIconHelper.Init();
        }

        public CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public PageControllHandler PageControllHandler { get; set; } = new PageControllHandler();
        public string Title { get; set; } = "StartPage";
        public BaseViewModel ViewModel { get; set; } = new StartViewModel();

        private async void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Image)sender).IsEnabled = false;

            var isConnected = ((StartViewModel)ViewModel).IsConnected =
                                !((StartViewModel)ViewModel).IsConnected;

            if (isConnected)
            {
                await ((StartViewModel)ViewModel).GetFtpListItems();
            }
            else
            {
                DirFileInfo.Visibility = Visibility.Hidden;
                DirFileInfo.DataContext = null;
                ((StartViewModel)ViewModel).FtpListItems?.Clear();
            }

            ((Image)sender).IsEnabled = true;
        }

        private async void TreeView_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = e.OriginalSource as TreeViewItem;
            if (tvi != null)
            {
                FileList.Cursor = Cursors.Wait;
                var ftpItmListM = (FtpListItemModel)tvi.DataContext;
                if (ftpItmListM.Type != FluentFTP.FtpFileSystemObjectType.File) await ((StartViewModel)ViewModel).GetFtpChildItems(ftpItmListM);

                FileList.Cursor = Cursors.Arrow;
            }
        }

        private async void TreeView_TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var textBlock = (TextBlock)sender;
                textBlock.Cursor = Cursors.Wait;
                var ftpItmListM = (FtpListItemModel)textBlock.DataContext;
                if (ftpItmListM.IsImage)
                {
                    ftpItmListM.GetImage();
                }
                if (ftpItmListM.IsOpk)
                {
                    var info = await ftpItmListM.GetOpkInfo();
                    this.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        opkName.Text = info?.Name ?? "";
                        opkComment.Text = info?.Comment ?? "";
                        opkType.Text = info?.Type ?? "";
                        opkImage.Source = info?.Image;
                    });
                }
                DirFileInfo.DataContext = ftpItmListM;
                DirFileInfo.Visibility = Visibility.Visible;
                spFileRename.Visibility = Visibility.Collapsed;
                spDirectoryRename.Visibility = Visibility.Collapsed;
                textBlock.Cursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                Console.WriteLine(ex);
                DirFileInfo.Visibility = Visibility.Hidden;
            }

            //if (ftpItmListM.Type != FluentFTP.FtpFileSystemObjectType.File) await ((StartViewModel)ViewModel).GetFtpChildItems(ftpItmListM);
        }

        private async void UpladFile_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            //button.Visibility = Visibility.Collapsed;
            UploadProgress.Visibility = Visibility.Visible;
            DirFileInfo.IsEnabled = false;
            FileList.IsEnabled = false;
            var ftpItmListM = (FtpListItemModel)button.DataContext;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog(Window.GetWindow(this)) == true)
            {
                var list = openFileDialog.FileNames;
                var ftpHelper = new FtpHelper();
                Progress<FtpProgress> progress = new Progress<FtpProgress>(p =>
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        if (p.Progress == 100 && p.FileCount - 1 == p.FileIndex)
                        {
                            ftpHelper.Progress = 100;
                            //button.Visibility = Visibility.Visible;
                            UploadProgress.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            ftpHelper.TransMessage = $"Upload from {p.LocalPath} to {p.RemotePath}";
                            ftpHelper.Progress = Math.Round(p.Progress, 1);
                            ftpHelper.FileCount = p.FileCount;
                            ftpHelper.FileIndex = p.FileIndex + 1;
                            ftpHelper.TransferSpeed = p.TransferSpeed;
                            ftpHelper.EAT = p.ETA.ToString(@"hh\:mm\:ss", null);
                            ftpHelper.TransferredBytes = p.TransferredBytes;
                            ftpHelper.TransferSpeedString = p.TransferSpeedToString();
                        }
                    });
                });
                UploadProgress.DataContext = ftpHelper;
                DirFileInfo.Visibility = Visibility.Hidden;
                cancellationTokenSource = new CancellationTokenSource();
                await ftpHelper.UploadFilesAsync(list, ftpItmListM.FullName, progress, cancellationTokenSource.Token);
                DirFileInfo.IsEnabled = true;
                FileList.IsEnabled = true;
                var holeDamThing = (StartViewModel)FileList.DataContext;
                var path = ftpItmListM.FullName;
                var dt = await holeDamThing.FindItemByPathAsync(ftpItmListM.FullName);
                if (dt != null)
                {
                    //var dt2 = await holeDamThing.FindItemByPathAsync(dt.ParentPath);
                    //await holeDamThing.GetFtpChildItems(dt2);
                    await holeDamThing.GetFtpListItems(dt.Items, dt.FullName, true);
                }
            }
        }

        private async void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var par = button.Parent;
            var ftpItmListM = (FtpListItemModel)button.DataContext;
            if (MessageBox.Show($"You are about to delete {(button.Name == "DeleteFile" ? "file" : "directory")} {ftpItmListM.Name} in {ftpItmListM.ParentPath}!\nThis can damage the os or make applications unusable!\nSo if you are not sure what you are doing, please do not proceed!\nDo you like to proceed?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            var ftpHelper = new FtpHelper();
            cancellationTokenSource = new CancellationTokenSource();
            if (button.Name == "DeleteFile") await ftpHelper.DeletedFileAsync(ftpItmListM.FullName, cancellationTokenSource.Token);
            else await ftpHelper.DeleteDirectoryAsync(ftpItmListM.FullName, cancellationTokenSource.Token);
            MessageBox.Show($"{(button.Name == "DeleteFile" ? "File" : "Directory")} {ftpItmListM.Name} deleted");
            var holeDamThing = (StartViewModel)FileList.DataContext;
            var path = ftpItmListM.FullName;
            var dt = await holeDamThing.FindItemByPathAsync(ftpItmListM.ParentPath);
            if (dt != null)
            {
                await holeDamThing.GetFtpListItems(dt.Items, dt.FullName, true);
            }
            DirFileInfo.Visibility = Visibility.Collapsed;
        }

        private void FileList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView item = e.OriginalSource as TreeView;

            if (item != null)

            {
                ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(item);

                if (parent != null)

                {
                    //Put your logic here.
                }
            }
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;

            var isConnected = ((StartViewModel)ViewModel).IsConnected =
                                !((StartViewModel)ViewModel).IsConnected;

            if (isConnected)
            {
                btnConnect.Content = "Disconnect";
                await ((StartViewModel)ViewModel).GetFtpListItems();
            }
            else
            {
                btnConnect.Content = "Connect";
                DirFileInfo.Visibility = Visibility.Hidden;
                DirFileInfo.DataContext = null;
                ((StartViewModel)ViewModel).FtpListItems?.Clear();
            }

            ((Button)sender).IsEnabled = true;
        }

        private void ScrapeFolder_Click(object sender, RoutedEventArgs e)
        {
            var page = PageHandler.GetPageByType<ScrapFolderView>();
            Button bt = (Button)sender;
            var ftpItmListM = (FtpListItemModel)bt.DataContext;
            ((ScrapFolderViewModel)page.ViewModel).FtpListItem = ftpItmListM;
            PageHandler.Instance.SetPage<ScrapFolderView>();
        }

        private void EditFile_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var par = button.Parent;
            var ftpItmListM = (FtpListItemModel)button.DataContext;
            var notPad = (NotePad)PageHandler.GetPageByType<NotePad>();
            notPad.LoadTextPath(ftpItmListM.FullName, ftpItmListM.Name);
            PageHandler.SelectedPage<NotePad>();
        }

        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.SelectAll();
            textBox.Focus();
        }

        private void btnCopyPath_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var par = button.Parent;
            var ftpItmListM = (FtpListItemModel)button.DataContext;
            Clipboard.SetText(ftpItmListM.FullName);
        }

        private void RenameFolder_Click(object sender, RoutedEventArgs e)
        {
            spDirectoryRename.Visibility = Visibility.Visible;
        }

        private void DirectoryRenameClose_Click(object sender, RoutedEventArgs e)
        {
            spDirectoryRename.Visibility = Visibility.Collapsed;
        }

        private async void DirectorySaveRename_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var par = button.Parent;
            var ftpItmListM = (FtpListItemModel)button.DataContext;
            var oldDirectory = ftpItmListM.mFullName;
            var newName = @"/" + ftpItmListM.Name;
            var newDirectoryName = ftpItmListM.ParentPath + newName;
            if (MessageBox.Show($"You are about to rename {oldDirectory} to {newDirectoryName}\nThis can damage the os or make applications unusable!\nSo if you are not sure what you are doing, please do not proceed!\nDo you like to proceed?", "Rename Directory?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;
            spDirectoryRename.Visibility = Visibility.Collapsed;
            var ftpHelper = new FtpHelper();
            cancellationTokenSource = new CancellationTokenSource();
            await ftpHelper.RenameDirectory(ftpItmListM.mFullName, newDirectoryName, cancellationTokenSource.Token);
            MessageBox.Show($"Rename directory ok");
            var holeDamThing = (StartViewModel)FileList.DataContext;
            var dt = await holeDamThing.FindItemByPathAsync(ftpItmListM.ParentPath);
            if (dt != null)
            {
                await holeDamThing.GetFtpListItems(dt.Items, dt.FullName, true);
            }
        }

        private void FileRenameClose_Click(object sender, RoutedEventArgs e)
        {
            spFileRename.Visibility = Visibility.Collapsed;
        }

        private async void FileSaveRename_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var par = button.Parent;
            var ftpItmListM = (FtpListItemModel)button.DataContext;
            var oldFilePath = ftpItmListM.mFullName;
            if (GeneralFunctions.IsFilenameInvalid(ftpItmListM.Name))
            {
                MessageBox.Show("Sorry Name Contains Invalid Chars!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            var newName = @"/" + ftpItmListM.Name;
            var newFileFullName = ftpItmListM.ParentPath + newName;
            if (MessageBox.Show($"You are about to rename {oldFilePath} to {newFileFullName}\nThis can damage the os or make applications unusable!\nSo if you are not sure what you are doing, please do not proceed!\nDo you like to proceed?", "Rename File?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;
            spFileRename.Visibility = Visibility.Collapsed;
            var ftpHelper = new FtpHelper();
            cancellationTokenSource = new CancellationTokenSource();
            await ftpHelper.RenameFile(ftpItmListM.mFullName, newFileFullName, cancellationTokenSource.Token);
            MessageBox.Show($"Rename File ok");
            var holeDamThing = (StartViewModel)FileList.DataContext;
            var dt = await holeDamThing.FindItemByPathAsync(ftpItmListM.ParentPath);
            if (dt != null)
            {
                await holeDamThing.GetFtpListItems(dt.Items, dt.FullName, true);
            }
        }

        private void RenameFile_Click(object sender, RoutedEventArgs e)
        {
            spFileRename.Visibility = Visibility.Visible;
        }

        private async void FileList_SelectedItemChanged_1(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var treeView = (TreeView)sender;
                var textBlock = treeView.SelectedItem;

                treeView.Cursor = Cursors.Wait;
                var ftpItmListM = (FtpListItemModel)textBlock;
                if (ftpItmListM.IsImage)
                {
                    ftpItmListM.GetImage();
                }
                if (ftpItmListM.IsOpk)
                {
                    var info = await ftpItmListM.GetOpkInfo();
                    this.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        opkName.Text = info?.Name ?? "";
                        opkComment.Text = info?.Comment ?? "";
                        opkType.Text = info?.Type ?? "";
                        opkImage.Source = info?.Image;
                    });
                }
                DirFileInfo.DataContext = ftpItmListM;
                DirFileInfo.Visibility = Visibility.Visible;
                treeView.Cursor = Cursors.Arrow;
                spFileRename.Visibility = Visibility.Collapsed;
                spDirectoryRename.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                Console.WriteLine(ex);
                DirFileInfo.Visibility = Visibility.Hidden;
            }
        }

        private async void DownloadFolder_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            //button.Visibility = Visibility.Collapsed;
            UploadProgress.Visibility = Visibility.Visible;
            DirFileInfo.IsEnabled = false;
            FileList.IsEnabled = false;
            var ftpItmListM = (FtpListItemModel)button.DataContext;
            var openFileDialog = new System.Windows.Forms.FolderBrowserDialog();
            openFileDialog.ShowNewFolderButton = true;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var path = openFileDialog.SelectedPath;
                var ftpHelper = new FtpHelper();
                Progress<FtpProgress> progress = new Progress<FtpProgress>(p =>
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        if (p.Progress == 100 && p.FileCount - 1 == p.FileIndex)
                        {
                            ftpHelper.Progress = 100;
                            //button.Visibility = Visibility.Visible;
                            UploadProgress.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            ftpHelper.TransMessage = $"Upload from {p.LocalPath} to {p.RemotePath}";
                            ftpHelper.Progress = Math.Round(p.Progress, 1);
                            ftpHelper.FileCount = p.FileCount;
                            ftpHelper.FileIndex = p.FileIndex + 1;
                            ftpHelper.TransferSpeed = p.TransferSpeed;
                            ftpHelper.EAT = p.ETA.ToString(@"hh\:mm\:ss", null);
                            ftpHelper.TransferredBytes = p.TransferredBytes;
                            ftpHelper.TransferSpeedString = p.TransferSpeedToString();
                        }
                    });
                });
                UploadProgress.DataContext = ftpHelper;
                cancellationTokenSource = new CancellationTokenSource();
                var res = await ftpHelper.DownloadDirectoryAsync(ftpItmListM.FullName, path, cancellationTokenSource.Token, progress, FtpFolderSyncMode.Mirror);
                //button.Visibility = Visibility.Visible;
                UploadProgress.Visibility = Visibility.Collapsed;
                DirFileInfo.IsEnabled = true;
                FileList.IsEnabled = true;
                var holeDamThing = (StartViewModel)FileList.DataContext;
                var dt = await holeDamThing.FindItemByPathAsync(ftpItmListM.FullName);
                if (dt != null)
                {
                    //var dt2 = await holeDamThing.FindItemByPathAsync(dt.ParentPath);
                    //await holeDamThing.GetFtpChildItems(dt2);
                    await holeDamThing.GetFtpListItems(dt.Items, dt.FullName, true);
                }
            }
        }

        private async void Downloafile_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            //button.Visibility = Visibility.Collapsed;
            UploadProgress.Visibility = Visibility.Visible;
            DirFileInfo.IsEnabled = false;
            FileList.IsEnabled = false;
            var ftpItmListM = (FtpListItemModel)button.DataContext;
            var openFileDialog = new System.Windows.Forms.SaveFileDialog();
            openFileDialog.FileName = ftpItmListM.Name;
            openFileDialog.Filter = "All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var path = openFileDialog.FileName;
                var ftpHelper = new FtpHelper();
                Progress<FtpProgress> progress = new Progress<FtpProgress>(p =>
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        if (p.Progress == 100)
                        {
                            ftpHelper.Progress = 100;
                            //button.Visibility = Visibility.Visible;
                            UploadProgress.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            ftpHelper.TransMessage = $"Upload from {p.LocalPath} to {p.RemotePath}";
                            ftpHelper.Progress = Math.Round(p.Progress, 1);
                            ftpHelper.FileCount = p.FileCount;
                            ftpHelper.FileIndex = p.FileIndex + 1;
                            ftpHelper.TransferSpeed = p.TransferSpeed;
                            ftpHelper.EAT = p.ETA.ToString(@"hh\:mm\:ss", null);
                            ftpHelper.TransferredBytes = p.TransferredBytes;
                            ftpHelper.TransferSpeedString = p.TransferSpeedToString();
                        }
                    });
                });
                UploadProgress.DataContext = ftpHelper;
                cancellationTokenSource = new CancellationTokenSource();
                var res = await ftpHelper.DownloadFileAsync(ftpItmListM.FullName, path, cancellationTokenSource.Token, progress);
                //button.Visibility = Visibility.Visible;
                UploadProgress.Visibility = Visibility.Collapsed;
                DirFileInfo.IsEnabled = true;
                FileList.IsEnabled = true;
                var holeDamThing = (StartViewModel)FileList.DataContext;
                var dt = await holeDamThing.FindItemByPathAsync(ftpItmListM.FullName);
                if (dt != null)
                {
                    //var dt2 = await holeDamThing.FindItemByPathAsync(dt.ParentPath);
                    //await holeDamThing.GetFtpChildItems(dt2);
                    await holeDamThing.GetFtpListItems(dt.Items, dt.FullName, true);
                }
            }
        }

        private void CanclUpload_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();

            //cancellationToken.ThrowIfCancellationRequested();
            cancellationTokenSource.Token.Register(() =>
            {
                cancellationTokenSource.Dispose();
                UploadProgress.Visibility = Visibility.Collapsed;
                DirFileInfo.IsEnabled = true;
                FileList.IsEnabled = true;
            }
            );
        }
    }

    //private string ReplaceLastOccurrence(string str, string toReplace, string replacement)
    //{
    //    return Regex.Replace(str, $@"^(.*){toReplace}(.*?)$", $"$1{replacement}$2");
    //}
}