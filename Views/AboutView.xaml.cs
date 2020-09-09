using RetroGameHandler.Handlers;
using RetroGameHandler.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RetroGameHandler.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl, IPage
    {
        public PageControllHandler PageControllHandler { get; set; } = new PageControllHandler();
        public string Title { get; set; } = "About";
        public BaseViewModel ViewModel { get; set; } = new BaseViewModel();

        public AboutView()
        {
            InitializeComponent();
            txtVersion.Content = RGHSettings.Version;

            licenstext.Text = Properties.Resources.LICENS;
        }

        private void download(object sender, RequestNavigateEventArgs e)
        {
        }

        private void nvgUri_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}