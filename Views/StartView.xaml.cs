using FluentFTP;
using Microsoft.Win32;
using RetroGameHandler.Handlers;
using RetroGameHandler.Helpers;
using RetroGameHandler.models;
using RetroGameHandler.ViewModels;
using System;
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
                textBlock.Cursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                DirFileInfo.Visibility = Visibility.Hidden;
            }

            //if (ftpItmListM.Type != FluentFTP.FtpFileSystemObjectType.File) await ((StartViewModel)ViewModel).GetFtpChildItems(ftpItmListM);
        }

        private async void UpladFile_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            button.Visibility = Visibility.Collapsed;
            UploadProgress.Visibility = Visibility.Visible;
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
                            button.Visibility = Visibility.Visible;
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
                var cancellationToken = new CancellationToken();
                await ftpHelper.UploadFilesAsync(list, ftpItmListM.FullName, progress, cancellationToken);
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
            if (MessageBox.Show($"Delete file {ftpItmListM.Name}?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            var ftpHelper = new FtpHelper();
            var cancellationToken = new CancellationToken();
            await ftpHelper.DeletedFileAsync(ftpItmListM.FullName, cancellationToken);
            MessageBox.Show("File deleted");
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
    }
}