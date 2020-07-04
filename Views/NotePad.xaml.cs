using Microsoft.Win32;
using RetroGameHandler.Handlers;
using RetroGameHandler.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RetroGameHandler.Views
{
    /// <summary>
    /// Interaction logic for NotePad.xaml
    /// </summary>
    public partial class NotePad : UserControl, IPage
    {
        public PageControllHandler PageControllHandler { get; set; } = new PageControllHandler();
        public string Title { get; set; } = "NotPage";
        public BaseViewModel ViewModel { get; set; } = new ScrapFolderViewModel();
        private ScrapFolderViewModel _vieModel { get; set; }
        private string _path = "";

        public NotePad()
        {
            InitializeComponent();
        }

        public void LoadTextPath(string path)
        {
            _path = path;
            txbPath.Text = _path;
            using (MemoryStream stream = new MemoryStream())
            {
                FtpHandler.Instance.DownloadStream(_path, stream);
                using (StreamReader reader = new StreamReader(stream))
                {
                    reader.BaseStream.Position = 0;
                    rtbEditor.Text = reader.ReadToEnd();
                    rtbEditor.Focus();
                    rtbEditor.SelectionStart = 0;
                    rtbEditor.SelectionLength = 0;
                    rtbEditor.Select(0, 1);
                }
            }
        }

        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text (*.txt)|*.txt|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                //FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                //TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                //range.Load(fileStream, DataFormats.Rtf);
            }
        }

        private async void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine(rtbEditor.Text);
                    if (await FtpHandler.Instance.UploadStreamAsync(stream, _path))
                    {
                        MessageBox.Show("Save OK", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Could not save!", "Save", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (cmbFontFamily.SelectedItem != null)
            //    rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
        }

        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            //rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            //string temp = txbSearch.Text;
            //txbSearch.Text = "";
            //txbSearch.Text = temp;
            var indx = rtbEditor.SelectionStart + rtbEditor.SelectionLength;
            if (indx == 0) indx = 1;
            index = rtbEditor.Text.IndexOf(txbSearch.Text, indx, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
            {
                index = rtbEditor.Text.IndexOf(txbSearch.Text, 0, StringComparison.OrdinalIgnoreCase);
            }
            if (index >= 0)
            {
                rtbEditor.Select(index, txbSearch.Text.Length);
                rtbEditor.SelectionBrush = Brushes.Yellow;
            }

            // index = rtbEditor.Text.IndexOf(txbSearch.Text, index) + 1;
        }
    }
}