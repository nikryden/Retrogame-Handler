using RetroGameHandler.Handlers;
using RetroGameHandler.Helpers;
using RetroGameHandler.UserControllImages;
using RetroGameHandler.Views;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace RetroGameHandler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ControllImages.Init();
            LiteDBHelper.init();
            TheGamesDbHandler.init();
            var asse = Assembly.GetEntryAssembly().GetName().Version;
            VersionText.Text = $"{asse.Major}.{asse.Minor}.{asse.Build}-beta";// Assembly.GetEntryAssembly().GetName().Version.ToString() + "-beta";
            RGHSettings.init();
            PageHandler.AddPage(new firstPage());
            PageHandler.AddPage(new RetroResorcesView());
            PageHandler.AddPage(new StartView());
            PageHandler.AddPage(new FtpSettingsView());
            PageHandler.AddPage(new OptionsView());
            PageHandler.AddPage(new ScrapFolderView());
            PageHandler.AddPage(new NotePad());
            PageHandler.AddPage(new LogView());

            this.DataContext = PageHandler.Instance;
            PageHandler.SelectedPage<firstPage>();
            //PageHandler.SelectedPage<LogView>();
            ConsoleIconHelper.Init();
            ConsoleIconHelper.Close += (s, e) =>
            {
                this.Close();
            };
            ConsoleIconHelper.OpenConsole += (s, e) =>
            {
                this.WindowState = WindowState.Normal;
            };
            //PageHandler.ThePageChanged += (s, e) =>
            //{
            //    ContentControl.Content = PageHandler.Instans.Page;
            //};
            //PageHandler.SelectedPage<FtpSettingsView>();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            PageHandler.SelectedPage<FtpSettingsView>();
        }

        private void StartPage_Click(object sender, RoutedEventArgs e)
        {
            PageHandler.SelectedPage<StartView>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //await FtpHandler.Instance.ConnectAsync();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ConsoleIconHelper.Dispose();
            BatteryIconHelper.Dispose();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void ToTray_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Title_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void mbtnMenueExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OtionsMenu_Click(object sender, RoutedEventArgs e)
        {
            PageHandler.SelectedPage<OptionsView>();
        }

        private void Resorces_Click(object sender, RoutedEventArgs e)
        {
            PageHandler.SelectedPage<RetroResorcesView>();
        }

        private void miHome_Click(object sender, RoutedEventArgs e)
        {
            PageHandler.SelectedPage<firstPage>();
        }

        private void btnPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            PageHandler.Instance.PreviousPage();
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            PageHandler.Instance.NextPage();
        }

        private void NotePadMenu_Click(object sender, RoutedEventArgs e)
        {
            PageHandler.SelectedPage<NotePad>();
        }

        private void LogPage_Click(object sender, RoutedEventArgs e)
        {
            PageHandler.SelectedPage<LogView>();
        }
    }
}