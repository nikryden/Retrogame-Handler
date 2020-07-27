using RetroGameHandler.Handlers;
using RetroGameHandler.Helpers;
using RetroGameHandler.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RetroGameHandler.Views.Modals
{
    /// <summary>
    /// Interaction logic for RenameView.xaml
    /// </summary>
    public partial class SearchView : Window
    {
        public DownloadImageModel DownloadImageModel;
        private DataModel _dataModel;
        private int _platformId;

        public SearchView(DataModel dataModel, int platformId)
        {
            InitializeComponent();
            _dataModel = dataModel;
            _platformId = platformId;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DownloadImageModel.Name = (string)searchResult.SelectedItem;// txbName.Text;
            DialogResult = true;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txbName.Text = DownloadImageModel.Name;
        }

        private async void btnTest_Click(object sender, RoutedEventArgs e)
        {
            TimeOnline.GameRoot game = null;

            try
            {
                game = await TheGamesDbHandler.GetGame(txbName.Text, _platformId, false, _dataModel.ScrapGuid);
                if (game == null || game.Games == null)
                {
                    searchResult.ItemsSource = new string[0];
                    MessageBox.Show("Found no games");
                    return;
                }

                searchResult.ItemsSource = game.Games.Select(g => g.GameTitle);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (game != null)
                {
                    _dataModel.ScrapGuid = game.UserInfo.NewGuid;
                    LiteDBHelper.Save(_dataModel);
                }
            }
        }
    }
}