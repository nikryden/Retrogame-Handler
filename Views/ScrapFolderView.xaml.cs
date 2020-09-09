using RetroGameHandler.Entities;
using RetroGameHandler.Handlers;
using RetroGameHandler.models;
using RetroGameHandler.thegamesAPI.Game1_1;
using RetroGameHandler.thegamesdbModel;
using RetroGameHandler.TimeOnline;
using RetroGameHandler.ViewModels;
using RetroGameHandler.Views.Modals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace RetroGameHandler.Views
{
    /// <summary>
    /// Interaction logic for ScrapFolder.xaml
    /// </summary>
    public partial class ScrapFolderView : UserControl, IPage
    {
        public PageControllHandler PageControllHandler { get; set; } = new PageControllHandler();
        public string Title { get; set; } = "ScrapPage";
        public BaseViewModel ViewModel { get; set; } = new ScrapFolderViewModel();
        private ScrapFolderViewModel _vieModel { get; set; }

        public ScrapFolderView()
        {
            InitializeComponent();
            _vieModel = (ScrapFolderViewModel)ViewModel;

            //_vieModel.PageLoadReady += (s, e) =>
            //{
            //    view = (CollectionView)CollectionViewSource.GetDefaultView(DownloadImageList.ItemsSource);
            //    view.GroupDescriptions.Add(new PropertyGroupDescription("Name"));
            //    view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            //    view.SortDescriptions.Add(new SortDescription("IsSelected", ListSortDirection.Descending));
            //};

            DataContext = ViewModel;
            this.Loaded += LoadOnce;
            this.Loaded += LoadPage;
        }

        private void LoadPage(object sender, RoutedEventArgs e)
        {
            //var data = LiteDBHelper.Load<DataModel>().FirstOrDefault() ?? new DataModel() { Id = 1, ScrapGuid = "", ScrapEmail = "" };
            //if (string.IsNullOrWhiteSpace(data.ScrapGuid))
            //{
            //    var login = new LoginView();
            //    login.ShowDialog();
            //}
        }

        private void LoadOnce(object sender, RoutedEventArgs e)
        {
            //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(DownloadImageList.ItemsSource);
            //view.GroupDescriptions.Add(new PropertyGroupDescription("Name"));
            //view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            //view.SortDescriptions.Add(new SortDescription("IsSelected", ListSortDirection.Descending));
            this.Loaded -= LoadOnce;
        }

        private void updateValues(int max, int value, string file)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                pbScraping.Maximum = max;
                pbScraping.Value = value;
                status.Text = $"{file}";
                percent.Text = $"{value + 1}/{max}";
            }));
        }

        private void updateDownloadList(int id, string name, string gameTitle, string fullname, string url, bool MultiImage = false, bool useDirectory = false, bool removeExisting = false)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (removeExisting)
                {
                    var itm = _vieModel.DownloadList.FirstOrDefault(l => l.FullName == fullname);
                    if (itm != null)
                    {
                        _vieModel.DownloadList.Remove(itm);
                    }
                    _vieModel.DownloadList.Add(new models.DownloadImageModel(id, name, gameTitle, fullname, url, MultiImage, useDirectory));
                    return;
                }
                var nm = useDirectory ? name : System.IO.Path.GetFileNameWithoutExtension(name);
                if (_vieModel.DownloadList.Any(g => g.GameTitle == gameTitle && g.Name == nm)) return;
                _vieModel.DownloadList.Add(new models.DownloadImageModel(id, nm, gameTitle, fullname, url, MultiImage, useDirectory));
            }));
        }

        private void updateDownloadList(int id, string name, string gameTitle, string fullname)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (_vieModel.DownloadList.Any(g => g.GameTitle == gameTitle && g.FullName == fullname)) return;
                _vieModel.DownloadList.Add(new models.DownloadImageModel(id, System.IO.Path.GetFileNameWithoutExtension(name), gameTitle, fullname));
            }));
        }

        private CollectionView view;

        private async Task updateDownloadImageList(bool show, char[] character = null)
        {
            if (character == null) character = new char[] { 'A' };
            await Dispatcher.BeginInvoke((Action)(async () =>
            {
                if (show)
                {
                    spPbScraping.Visibility = Visibility.Visible;
                    pbScraping.IsIndeterminate = true;
                    status.Text = "Filtering...";
                    this.Cursor = System.Windows.Input.Cursors.Wait;
                    this.IsEnabled = false;
                    spDownloadImageList.Visibility = Visibility.Visible;
                    _vieModel.Character = character;
                    //await Dispatcher.BeginInvoke((Action)(async () =>
                    // {
                    DownloadImageList.ItemsSource = await _vieModel.GetPage();
                    //}));
                    view = (CollectionView)CollectionViewSource.GetDefaultView(DownloadImageList.ItemsSource);
                    view.GroupDescriptions.Add(new PropertyGroupDescription("FullName"));
                    view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                    view.SortDescriptions.Add(new SortDescription("IsSelected", ListSortDirection.Descending));
                    this.Cursor = System.Windows.Input.Cursors.Arrow;
                    this.IsEnabled = true;
                    spPbScraping.Visibility = Visibility.Collapsed;
                    status.Text = "";
                    pbScraping.IsIndeterminate = false;
                }
                else
                {
                    spDownloadImageList.Visibility = Visibility.Hidden;
                }
            }));
        }

        private int _countAll = 0;
        public bool _cancelScrap = false;

        private async Task ScrapItem(FtpListItemModel item, int PlatformId, bool useDirectory, bool removeIfExist = false)
        {
            var name = (useDirectory ? item.Name : System.IO.Path.GetFileNameWithoutExtension(item.Name)) + ".png";
            var filePathConsole = $@"{_vieModel.FtpListItem.FullName }/media/{name}";
            item.LastSearchParam = useDirectory ? item.ParentPath.Split('/').Last() : item.Name;
            var game = await TheGamesDbHandler.GetGame(item.LastSearchParam, PlatformId, useDirectory, data.ScrapGuid);

            if (game.Games == null || errorHandling(game.Error, 0))
            {
                try
                {
                    if (game.Error != null && game.Error.Code == 200)
                    {
                        data.ScrapGuid = game.UserInfo.NewGuid;
                        Debug.WriteLine($"game.Error.Message");

                        updateDownloadList(0, item.Name, "", item.mFullName, "", false);
                    }
                    else return;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                try
                {
                    data.ScrapGuid = game.UserInfo.NewGuid;
                    _countAll++;
                    foreach (var gme in game.Games)
                    {
                        var boxart = gme.BoxArts.FirstOrDefault();

                        if (boxart == null) continue;
                        var filePath = boxart.Filename;//game.BaseUrls.FirstOrDefault(b => b.Name == "small").Path +
                        var path = filePath;
                        Debug.WriteLine($"Found:{ item.Name} ({gme.GameTitle}) path: {path}");
                        updateDownloadList(gme.Id, useDirectory ? item.ParentPath.Split('/').Last() : item.Name,
                            gme.GameTitle, item.mFullName, path, game.Games.Count > 1, useDirectory, removeIfExist);
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.Error(ex);
                    return;
                }
            }
        }

        private IEnumerable<string> getExistingImages = new List<string>();

        private bool skipIfexists = false;

        private async Task ScrapingSearch(FtpListItemModel ftpListItem, bool useDirectory, bool isChild = false)
        {
            await Task.Run(async () =>
            {
                try
                {
                    if (!isChild) await updateDownloadImageList(false);
                    var PlatformId = _vieModel.SelectedPlatform.Id;
                    var itemsCount = 0;

                    if (ftpListItem.HasChild)
                    {
                        if (ftpListItem.Items.Any(i => i == null)) await ftpListItem.GetChild(true);
                        //itemsCount = ftpListItem.Items.Count;

                        foreach (var item in ftpListItem.Items.Where(f => !skipIfexists || (skipIfexists && !getExistingImages.Contains(Path.GetFileNameWithoutExtension(f.Name)))))
                        {
                            if (item == null) return;//ToDo;if null load list
                            if (item.IsFile && _vieModel.SelectedPlatform.Extensions.Contains(System.IO.Path.GetExtension(item.Name), StringComparer.OrdinalIgnoreCase))
                            {
                                if (skipIfexists && getExistingImages.Contains(Path.GetFileNameWithoutExtension(item.Name)))
                                {
                                    continue;
                                }
                                if (_cancelScrap) break;
                                await ScrapItem(item, PlatformId, useDirectory);
                            }
                            else if (item.IsDirectory)
                            {
                                await ScrapingSearch(item, useDirectory, true);
                            }

                            updateValues(ftpListItem.Items.Count, itemsCount++, item.Name);
                        }

                        if (!isChild)
                        {
                            var lst = _vieModel.DownloadList?.OrderBy(g => g.Name).Where(l => !string.IsNullOrWhiteSpace(l.Name) && _vieModel.HaveImages(l.Name)).ToList() ?? new List<DownloadImageModel>();
                            foreach (var item in lst)
                            {
                                if (item.IsSelected && lst.Any(l => l.IsSelected && l.Name == item.Name && l.GameTitle != item.GameTitle))
                                    item.IsSelected = false;
                                if (!item.IsSelected && lst.Count(g => g.Name == item.Name) == 1) item.IsSelected = true;
                            }

                            await updateDownloadImageList(true, await UpdateChar());
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.Error(ex);
                    Console.WriteLine(ex);
                    return;
                }
            });
        }

        private async Task<char[]> UpdateChar()
        {
            char[] firstChar = new char[] { 'A' };
            await Dispatcher.BeginInvoke((Action)(() =>
            {
                var isFirst = false;
                foreach (var item in spprewController.Children.OfType<TextBlock>())
                {
                    if (!item.Name.StartsWith("t")) continue;
                    item.Background = System.Windows.Media.Brushes.Transparent;
                    var ch = new char[] { 'A' };
                    if (item.Text == "0-9") ch = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                    else if (item.Name == "tNoSelected")
                    {
                        continue;
                    }
                    else if (item.Name == "tNotFound")
                    {
                        continue;
                    }
                    else
                    {
                        ch = new char[] { item.Text[0] };
                    }
                    var haveData = _vieModel.HaveData(ch);
                    if (haveData && !isFirst)
                    {
                        item.Background = System.Windows.Media.Brushes.Coral;
                        firstChar = ch;
                        isFirst = true;
                    }
                    item.IsEnabled = haveData;
                }
            }));
            return firstChar;
        }

        private DataModel data;

        private bool errorHandling(Error error, int count)
        {
            try
            {
                if (error == null) return true;
                if (error.Code > 401)
                {
                    MessageBox.Show(error.Message, "Scrapping error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return true;
                }
                else if (error.Code == 401)
                {
                    return !Login(count);
                }
                return error.Code > 299;
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                Debug.WriteLine(ex);
                return true;
            }
        }

        private bool Login(int count = 0)
        {
            try
            {
                if (count >= 3) return false;
                var login = new LoginView();
                login.ShowDialog();
                if (!login.DialogResult.Value) return false;
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    data = LiteDBHelper.Load<DataModel>().FirstOrDefault() ?? new DataModel() { Id = 1, ScrapGuid = "", ScrapEmail = "" };
                    var burl = new GameRoot();
                    var task = Task.Run(async () =>
                    {
                        burl = await TheGamesDbHandler.GetGame("¤", 1, false, data.ScrapGuid);
                    });
                    task.Wait();

                    _ = errorHandling(burl.Error, count);

                    data.ScrapGuid = burl.UserInfo.NewGuid;
                    LiteDBHelper.Save(data);
                    TheGamesDbHandler.BaseUrls = burl.BaseUrls;
                }));
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                return false;
            }
        }

        private async void btnScrap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                spPbScraping.Visibility = Visibility.Visible;
                btnScrap.IsEnabled = false;
                spUploadSettings.IsEnabled = false;
                _vieModel.DownloadList.Clear();
                _countAll = 0;
                data = LiteDBHelper.Load<DataModel>().FirstOrDefault() ?? new DataModel() { Id = 1, ScrapGuid = "", ScrapEmail = "" };
                var burl = await TheGamesDbHandler.GetGame("timeonlin9022", 1, false, data.ScrapGuid);
                if (errorHandling(burl.Error, 0))
                {
                    spPbScraping.Visibility = Visibility.Collapsed;
                    status.Text = "";
                    percent.Text = "";
                    btnScrap.IsEnabled = true;
                    spUploadSettings.IsEnabled = true;
                    return;
                }
                else
                {
                    data.ScrapGuid = burl.UserInfo.NewGuid;
                    TheGamesDbHandler.BaseUrls = burl.BaseUrls;
                }
                //
                _cancelScrap = false;
                getExistingImages = FtpHandler.Instance.List(_vieModel.FtpListItem.mFullName + "/media").Where(f => f.Name.EndsWith(".png")).Select(f => Path.GetFileNameWithoutExtension(f.Name));
                skipIfexists = cbSkipExist.IsChecked.Value;
                //_ = Dispatcher.BeginInvoke((Action)(() => skipIfexists = cbSkipExist.IsChecked.Value));
                await ScrapingSearch(_vieModel.FtpListItem, cbUseDirectory.IsChecked.Value);
                spPbScraping.Visibility = Visibility.Collapsed;
                status.Text = "";
                percent.Text = "";
                btnScrap.IsEnabled = true;
                spUploadSettings.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex);
                Debug.WriteLine(ex);
            }
            finally
            {
                LiteDBHelper.Save(data);
            }
        }

        private async void btnUploadToConsole_Click(object sender, RoutedEventArgs e)
        {
            spUploadSettings.IsEnabled = false;
            btnScrap.IsEnabled = false;
            DownloadImageList.IsEnabled = false;
            spprewController.IsEnabled = false;
            _vieModel.FtpHelper.PropertyChanged += (s, ea) =>
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    pbUpload.Value = _vieModel.FtpHelper.Progress;
                    pbUploadStaus.Text = _vieModel.FtpHelper.TransMessage;
                }));
            };

            var c = spUploadSettings.Children.OfType<RadioButton>().FirstOrDefault(r => r.GroupName == "grpSize" && r.IsChecked.Value)?.Tag?.ToString() ?? "orginal";

            var pathName = c == "custom" ? "orginal" : c;

            var path = TheGamesDbHandler.BaseUrls.FirstOrDefault(s => s.Name == pathName)?.Path;
            if (path == null) return;
            var list = _vieModel.DownloadList.Where(l => l.IsSelected);
            var count = 0;
            pbUpload2.Maximum = list.Count();

            foreach (var item in list.ToList())
            {
                count++;
                uploadStatus.Text = $"{count} / {pbUpload2.Maximum}";
                pbUpload2.Value = count;

                var Url = path + item.DownloadPath;
                byte[] ImageStream;
                await Task.Run(async () =>
                {
                    using (WebClient client = new WebClient())
                    {
                        ImageStream = client.DownloadData(Url);
                    }
                    using (MemoryStream mem = new MemoryStream(ImageStream))
                    {
                        var name = Path.GetFileNameWithoutExtension(item.FullName) + ".png";
                        var filePathConsole = $@"{_vieModel.FtpListItem.FullName }/media/{name}";
                        var isok = await _vieModel.FtpHelper.UploadStreamAsync(mem, filePathConsole);
                        if (isok)
                        {
                            await Task.Run(() =>
                            {
                                IReadOnlyList<DownloadImageModel> itemToRemove = _vieModel.DownloadList.Where(x => (x.Name == item.Name)).
                                                                           ToList();
                                foreach (var itm in itemToRemove)
                                {
                                    _vieModel.DownloadList.Remove(itm);
                                }
                            });
                        }
                        else
                        {
                            return;
                        }
                    }
                });
            }
            //lock (_vieModel.DownloadList)
            //{
            //foreach (var ritem in remove)
            //{
            //    IReadOnlyList<DownloadImageModel> usersToRemove = _vieModel.DownloadList.Where(x => (x.Name == ritem)).
            //                             ToList();
            //    foreach (var itm in usersToRemove)
            //    {
            //        _vieModel.DownloadList.Remove(itm);
            //    }
            //}
            //}
            await updateDownloadImageList(true, await UpdateChar());
            spUploadSettings.IsEnabled = true;
            btnScrap.IsEnabled = true;
            DownloadImageList.IsEnabled = true;
            spprewController.IsEnabled = true;
            //sbUpload
        }

        private async void tbxAlfa_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (var item in spprewController.Children.OfType<TextBlock>())
            {
                if (!item.Name.StartsWith("t")) continue;
                item.Background = System.Windows.Media.Brushes.Transparent;
            }
            var txb = (TextBlock)sender;
            txb.Background = System.Windows.Media.Brushes.Coral;

            var tx = txb.Text;
            var ch = new char[] { 'A' };
            if (tx == "0-9") ch = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            else if (txb.Name == "tNoSelected")
            {
                ch = new char[] { '?' };
            }
            else if (txb.Name == "tNotFound")
            {
                ch = new char[] { '!' };
            }
            else
            {
                ch = new char[] { tx[0] };
            }
            await updateDownloadImageList(true, ch);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnScrap.IsEnabled = true;
        }

        private void btnCancelScrap_Click(object sender, RoutedEventArgs e)
        {
            _cancelScrap = true;
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var sendr = (MenuItem)sender;
            var dc = (CollectionViewGroup)sendr.DataContext;
            var nm = dc.Name.ToString();

            var item = (DownloadImageModel)dc.Items[0];// _vieModel.FtpListItem.Items.FirstOrDefault(s => s.Name.StartsWith(nm));
            if (item == null) return;
            var rnView = new RenameView(data, _vieModel.SelectedPlatform.Id);
            rnView.Title = $" Rename game";
            rnView.Owner = Window.GetWindow(this);
            rnView.Icon = _vieModel.SelectedPlatform.BitmapImage;
            rnView.DownloadImageModel = item;
            var res = rnView.ShowDialog();
            if (res.HasValue && res.Value)
            {
                var ftpItem = FindFtpItem(_vieModel.FtpListItem.Items.FirstOrDefault(s => s.FullName == item.FullName || item.FullName.Contains(s.FullName)));
                if (ftpItem == null) return;
                item.FullName = item.FullName.Replace(Path.GetFileNameWithoutExtension(ftpItem.Name), item.Name);
                await ftpItem.RenameFile(item.Name + Path.GetExtension(item.FullName));

                var PlatformId = _vieModel.SelectedPlatform.Id;
                await ScrapItem(ftpItem, PlatformId, false);
                LiteDBHelper.Save(data);
                await UpdateChar();
                var ch = new char[] { '!' };
                await updateDownloadImageList(true, ch);
            }
        }

        private void MenuItem_Click_Copy(object sender, RoutedEventArgs e)
        {
            var sendr = (MenuItem)sender;
            var dc = (CollectionViewGroup)sendr.DataContext;
            var nm = dc.Name.ToString();
            System.Windows.Clipboard.SetText(nm);
            MessageBox.Show("Copy to clipboard ok", "Copy", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            //var sendr = (RadioButton)sender;
            //var dc = (DownloadImageModel)sendr.DataContext;
            ////dc.IsSelected = !dc.IsSelected;
            //sendr.IsChecked = !sendr.IsChecked;
        }

        private async void MenuItem_Click_Search(object sender, RoutedEventArgs e)
        {
            var sendr = (MenuItem)sender;
            var dc = (CollectionViewGroup)sendr.DataContext;
            var nm = dc.Name.ToString();

            var item = (DownloadImageModel)dc.Items[0];// _vieModel.FtpListItem.Items.FirstOrDefault(s => s.Name.StartsWith(nm));
            if (item == null) return;
            var rnView = new SearchView(data, _vieModel.SelectedPlatform.Id);
            rnView.Title = $" Search for game";
            rnView.Owner = Window.GetWindow(this);
            rnView.Icon = _vieModel.SelectedPlatform.BitmapImage;
            rnView.DownloadImageModel = item;
            var res = rnView.ShowDialog();
            if (res.HasValue && res.Value)
            {
                var ftpItem = FindFtpItem(_vieModel.FtpListItem.Items.FirstOrDefault(s => s.FullName == item.FullName || item.FullName.Contains(s.FullName)));

                if (ftpItem == null) return;
                ftpItem.Name = item.Name;
                //item.FullName = item.FullName.Replace(Path.GetFileNameWithoutExtension(ftpItem.Name), item.Name);
                var PlatformId = _vieModel.SelectedPlatform.Id;
                await ScrapItem(ftpItem, PlatformId, false, true);
                LiteDBHelper.Save(data);
                await UpdateChar();
                var ch = new char[] { '!' };
                await updateDownloadImageList(true, ch);
            }
        }

        private void MenuItem_Click_Info(object sender, RoutedEventArgs e)
        {
            var sendr = (MenuItem)sender;
            var dc = (CollectionViewGroup)sendr.DataContext;
            var nm = dc.Name.ToString();

            var item = (DownloadImageModel)dc.Items[0];
            var vm = FindFtpItem(_vieModel.FtpListItem.Items.FirstOrDefault(i => item.FullName.Contains(i.FullName)));
            var message = $"File path:{item.FullName}\nSearch:{vm.LastSearchParam}";
            MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private FtpListItemModel FindFtpItem(FtpListItemModel item)
        {
            if (item == null) return null;
            if (item.IsDirectory) return FindFtpItem(item.Items.FirstOrDefault(i => i.FullName.Contains(item.FullName)));
            else return item;
        }

        private void ComboBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var cb = (ComboBox)sender;
            foreach (PlatformModel i in cb.Items)
            {
                if (i.Name.ToUpper().StartsWith(e.Text.ToUpper()))
                {
                    cb.SelectedItem = i;
                    break;
                }
            }
            e.Handled = true;
        }

        private void MenuItem_Click_CopyTitle(object sender, RoutedEventArgs e)
        {
            var sendr = (MenuItem)sender;
            var dc = (CollectionViewGroup)sendr.DataContext;
            var nm = (DownloadImageModel)dc.Items[0];
            System.Windows.Clipboard.SetText(nm.Name);
            MessageBox.Show("Copy to clipboard ok", "Copy", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}