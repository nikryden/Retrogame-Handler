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
    public partial class RenameView : Window
    {
        public DownloadImageModel DownloadImageModel;
        private DataModel _dataModel;
        private int _platformId;

        public RenameView(DataModel dataModel, int platformId)
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
            var fName = txbName.Text;
            if (GeneralFunctions.IsFilenameInvalid(fName))
            {
                MessageBox.Show("Sorry Name Contains Invalid Chars!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // + txExtention.Text;
            //await DownloadImageModel.RenameFile(fName);
            if (MessageBox.Show($"You are about to rename {DownloadImageModel.Name + txExtention.Text} to {fName + txExtention.Text}\nThis can damage the os or make applications unusable!\nSo if you are not sure what you are doing, please do not proceed!\nDo you like to proceed?", "Rename File?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;
            DownloadImageModel.Name = fName;
            DialogResult = true;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txbName.Text = DownloadImageModel.Name;
            txExtention.Text = System.IO.Path.GetExtension(DownloadImageModel.FullName);
        }

        private async void btnTest_Click(object sender, RoutedEventArgs e)
        {
            TimeOnline.GameRoot game = null;
            if (GeneralFunctions.IsFilenameInvalid(txbName.Text))
            {
                MessageBox.Show("Sorry Name Contains Invalid Chars!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            try
            {
                game = await TheGamesDbHandler.GetGame(txbName.Text, _platformId, false, _dataModel.ScrapGuid);
                if (game == null || game.Games == null)
                {
                    MessageBox.Show("Found no games");
                    return;
                }
                var listGames = game.Games.Select(g => g.GameTitle);
                MessageBox.Show($"Found {game.Games.Count()} games.\n[{string.Join("],\n[", listGames.Take(5))}]{(listGames.Count() > 5 ? "\n...\nThe result is large, please improve the game name" : "")}", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
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