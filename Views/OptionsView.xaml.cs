using RetroGameHandler.Handlers;
using RetroGameHandler.models;
using RetroGameHandler.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace RetroGameHandler.Views
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class OptionsView : UserControl, IPage
    {
        public OptionsView()
        {
            InitializeComponent();
            ViewModel = RGHSettings.ProgramSetting;
            this.DataContext = ViewModel;
        }

        public PageControllHandler PageControllHandler { get; set; } = new PageControllHandler();
        public string Title { get; set; } = "StartPage";
        public BaseViewModel ViewModel { get; set; } = new RGHSettingsModel();

        private void btnAddFtpSettings_Click(object sender, RoutedEventArgs e)
        {
            RGHSettings.newFtpSetting("New Setting #N");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            RGHSettings.Save();
        }
    }
}