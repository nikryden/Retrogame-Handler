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
            VersionText.Text = Assembly.GetEntryAssembly().GetName().Version.ToString() + "_Alpha";
            RGHSettings.init();
            PageHandler.AddPage(new RetroResorcesView());
            PageHandler.AddPage(new StartView());
            PageHandler.AddPage(new FtpSettingsView());
            PageHandler.AddPage(new OptionsView());
           
            this.DataContext = PageHandler.Instance;
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
    }
}