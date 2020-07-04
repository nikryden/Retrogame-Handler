using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
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
        private readonly SearchPanel searchPanel;

        public NotePad()
        {
            InitializeComponent();
            searchPanel = SearchPanel.Install(aEditor);
            this.Loaded += LoadedOnce;
        }

        private void LoadedOnce(object sender, RoutedEventArgs e)
        {
            var hl = new HighlightingManager();
            var def = hl.HighlightingDefinitions;

            aEditor.ShowLineNumbers = true;
            this.Loaded -= LoadedOnce;
        }

        public static System.Drawing.Color IntToColor(int rgb)
        {
            return System.Drawing.Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        public void LoadTextPath(string path, string name)
        {
            _path = path;
            txbPath.Text = _path;
            aEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(System.IO.Path.GetExtension(name));
            using (MemoryStream stream = new MemoryStream())
            {
                FtpHandler.Instance.DownloadStream(_path, stream);
                stream.Position = 0;
                aEditor.Load(stream);
                //using (StreamReader reader = new StreamReader(stream))
                //{
                //    reader.BaseStream.Position = 0;
                //    rtbEditor.Text = reader.ReadToEnd();
                //    aEditor.Text = rtbEditor.Text;
                //    rtbEditor.Focus();
                //    rtbEditor.SelectionStart = 0;
                //    rtbEditor.SelectionLength = 0;
                //    rtbEditor.Select(0, 1);
                //}
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
            if (MessageBox.Show($"Do you like to save {_path}", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            using (MemoryStream stream = new MemoryStream())
            {
                aEditor.Save(stream);
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
            searchPanel.Open();
        }

        private void SearchString(object sender, RoutedEventArgs e)
        {
            var txt = aEditor.SelectedText;
            searchPanel.SearchPattern = txt ?? "";
            searchPanel.Open();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }

    public static class Commands
    {
        public static readonly RoutedUICommand SearchString = new RoutedUICommand("Search ", "SearchString", typeof(NotePad));
    }
}