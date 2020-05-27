using RetroGameHandler.Handlers;
using RetroGameHandler.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RetroGameHandler.Views
{
    /// <summary>
    /// Interaction logic for FtpSettingsView.xaml
    /// </summary>
    public partial class FtpSettingsView : UserControl, IPage

    {
        public FtpSettingsView()
        {
            InitializeComponent();
            this.DataContext = (FtpSettingsViewModels)ViewModel;
        }

        public BaseViewModel ViewModel { get; set; } = new FtpSettingsViewModels();
        public PageControllHandler PageControllHandler { get; set; } = new PageControllHandler();
        public string Title { get; set; } = "Ftp Settings";

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //var viewModel = (FtpSettingsViewModels)ViewModel;
            //var lst = (await viewModel.Connect()).ToList();
            //await viewModel.FTP.ChangeDirectoryAsync("/media/data/apps");
            //var lst2 = (await viewModel.FTP.List()).ToList();
            //Console.WriteLine(lst2.Count());

            var rs = new RestHandler();
            var games = rs.GetGameByName("Alundra 2");
            foreach (var game in games.Data.Games)
            {
                if (game.GameTitle == "Alundra")
                {
                    var bxArt = games.Include.Boxarts.data.FirstOrDefault(b => b.Key == game.Id);

                    //var id = game.Id;
                    //var img = rs.GetGameImage(id);
                    var baseUrl = games.Include.Boxarts.BaseUrl.Thumb;
                    //var path = img.Data.Images.FirstOrDefault(i => i.Type == "boxart");
                    //Console.WriteLine(baseUrl + path.Filename);
                    ImgBrowser.Source = new Uri(baseUrl + bxArt.Value.First(i => i.Side == "front").Filename);
                }
            }
        }
    }
}