using RetroGameHandler.Handlers;
using RetroGameHandler.ViewModels;
using RetroGameHandler.Views.Modals;
using System;
using System.Diagnostics;
using System.IO;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RetroGameHandler.Views
{
    /// <summary>
    /// Interaction logic for firstPage.xaml
    /// </summary>
    public partial class firstPage : UserControl, IPage
    {
        public PageControllHandler PageControllHandler { get; set; } = new PageControllHandler();
        public string Title { get; set; } = "Resorces";
        public BaseViewModel ViewModel { get; set; } = new RetroResorcesViewModel();

        public firstPage()
        {
            InitializeComponent();
            DownloadImage();
        }

        private void btnDriverInstall_Click(object sender, RoutedEventArgs e)
        {
            driverInstall();
        }

        private void DownloadImage()
        {
            Uri resourceUri = new Uri("http://timeonline.se/RGHandler/images/info.png", UriKind.Absolute);
            imgInfo.Source = new BitmapImage(resourceUri);
        }

        private void driverInstall()
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.FileName = "cmd.exe";
            var driverPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"RG350-signed_driver");
            process.StartInfo.Arguments = "/c C:\\Windows\\System32\\InfDefaultInstall.exe " + driverPath; // where driverPath is path of .inf file
            process.Start();
            process.WaitForExit();
            process.Dispose();
            MessageBox.Show(@"Driver has been installed");
        }

        private void btnResorces_Click(object sender, RoutedEventArgs e)
        {
            PageHandler.SelectedPage<RetroResorcesView>();
        }

        private void btnExplorer_Click(object sender, RoutedEventArgs e)
        {
            PageHandler.SelectedPage<StartView>();
        }

        private void btnOtionsMenu_Click(object sender, RoutedEventArgs e)
        {
            PageHandler.SelectedPage<OptionsView>();
        }

        private void btnOtionsHelp_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginView();
            login.ShowDialog();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}